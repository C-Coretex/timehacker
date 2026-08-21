using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.Observability;

namespace TimeHacker.Domain.Services.Processors;

public class TaskTimelineProcessor: ITaskTimelineProcessor
{
    public TasksForDayReturn GetTasksForDay(IEnumerable<FixedTask> fixedTasks, IEnumerable<FixedTask> scheduledFixedTasks, IEnumerable<DynamicTask> dynamicTasks, IEnumerable<Category> categories, DateOnly date)
    {
        var returnData = new TasksForDayReturn()
        {
            Date = date,
        };

        // Categories are a passive backdrop: they are laid out independently and never consume time, so
        // dynamic-task gap-filling below still sees only the task timeline.
        returnData.CategoriesTimeline.AddRange(GetCategoriesTimeline(categories));

        var fixedTasksTimeline = GetFixedTasksTimeline(fixedTasks, date);
        returnData.TasksTimeline.AddRange(fixedTasksTimeline);

        fixedTasksTimeline = GetFixedTasksTimeline(scheduledFixedTasks, date);
        returnData.TasksTimeline.AddRange(fixedTasksTimeline);

        var timeRanges = returnData.TasksTimeline.Select(tt => tt.TimeRange);
        var dynamicTaskCandidates = dynamicTasks.ToList();
        var dynamicTasksTimeline = GetDynamicTasksTimeline(dynamicTaskCandidates, timeRanges);
        returnData.TasksTimeline.AddRange(dynamicTasksTimeline);

        returnData = returnData with
        {
            TasksTimeline = returnData.TasksTimeline.OrderBy(t => t.TimeRange.Start).ToList(),
            CategoriesTimeline = returnData.CategoriesTimeline.OrderBy(c => c.TimeRange.Start).ToList()
        };

        RecordSchedulingQuality(dynamicTaskCandidates, returnData);
        return returnData;
    }

    /// <summary>
    /// Reports how well the generated day turned out: how many of the offered dynamic tasks actually found a
    /// gap, and how much of the day ended up occupied. Together these show whether the scheduler is starving
    /// tasks or leaving the day empty — neither is visible from duration or count metrics alone.
    /// </summary>
    private static void RecordSchedulingQuality(IList<DynamicTask> dynamicTaskCandidates, TasksForDayReturn tasksForDay)
    { // TODO: Probably we will get rid of this metric
        var placedTaskIds = tasksForDay.TasksTimeline
            .Where(t => !t.IsFixed)
            .Select(t => t.Task.Id)
            .ToHashSet();

        var placed = dynamicTaskCandidates.Count(t => placedTaskIds.Contains(t.Id));

        TimeHackerTelemetry.DynamicTasksScheduled.Add(placed,
            new KeyValuePair<string, object?>(TimeHackerTelemetry.OutcomeTagName, TimeHackerTelemetry.OutcomePlaced));
        TimeHackerTelemetry.DynamicTasksScheduled.Add(dynamicTaskCandidates.Count - placed,
            new KeyValuePair<string, object?>(TimeHackerTelemetry.OutcomeTagName, TimeHackerTelemetry.OutcomeUnplaced));

        var occupied = tasksForDay.TasksTimeline.Sum(t => (t.TimeRange.End - t.TimeRange.Start).TotalMinutes);
        // Ranges can overlap (a fixed task may sit inside another), so clamp rather than report above 1.
        TimeHackerTelemetry.DayUtilization.Record(Math.Clamp(occupied / TimeSpan.FromDays(1).TotalMinutes, 0, 1));
    }

    /// <summary>
    /// Turns each category into the time window it occupies on this day. Categories may freely overlap each
    /// other — several can cover the same hour — so no de-duplication or conflict resolution happens here.
    /// </summary>
    private static IEnumerable<CategoryContainerReturn> GetCategoriesTimeline(IEnumerable<Category> categories)
    {
        return categories.Select(category => new CategoryContainerReturn()
        {
            Category = category,
            ScheduleEntityId = category.ScheduleEntityId,
            TimeRange = new TimeRange(category.StartTime.ToTimeSpan(), category.EndTime.ToTimeSpan())
        });
    }

    private static IEnumerable<TaskContainerReturn> GetFixedTasksTimeline(IEnumerable<FixedTask> fixedTasks, DateOnly date)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = date.ToDateTime(TimeOnly.MaxValue);

        return fixedTasks.Select(fixedTask =>
        {
            // Clamp multi-day / cross-midnight tasks to the current day so the time range
            // never inverts (end < start), which would corrupt the dynamic-gap math.
            var start = fixedTask.StartTimestamp < dayStart ? DaytimeConstants.StartOfDay : fixedTask.StartTimestamp.TimeOfDay;
            var end = fixedTask.EndTimestamp > dayEnd ? DaytimeConstants.EndOfDay : fixedTask.EndTimestamp.TimeOfDay;

            return new TaskContainerReturn()
            {
                Task = fixedTask,
                IsFixed = true,
                ScheduleEntityId = fixedTask.ScheduleEntityId,
                TimeRange = new TimeRange(start, end)
            };
        });
    }

    private static IEnumerable<TaskContainerReturn> GetDynamicTasksTimeline(IList<DynamicTask> dynamicTasks, IEnumerable<TimeRange> timeRanges)
    {
        var startTimeSpan = DaytimeConstants.StartOfDay;
        TimeRange timeRange;
        var dynamicTasksTimeline = new List<TaskContainerReturn>();
        foreach (var takenTimeRange in timeRanges)
        {
            timeRange = new TimeRange(startTimeSpan, takenTimeRange.Start - DaytimeConstants.TimeBacklashBetweenTasks);

            if (timeRange.Start < timeRange.End)
            {
                var tasks = GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
                dynamicTasksTimeline.AddRange(tasks);
            }

            startTimeSpan = takenTimeRange.End + DaytimeConstants.TimeBacklashBetweenTasks;
        }

        timeRange = new TimeRange(startTimeSpan, DaytimeConstants.EndOfDay);
        if (timeRange.Start < timeRange.End)
        {
            var tasks = GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
            dynamicTasksTimeline.AddRange(tasks);
        }

        //for now hard limit count of tasks for easier debugging
        //TODO: remove it (replace with external solver)
        return dynamicTasksTimeline.Shuffle().Take(10).OrderBy(t => t.TimeRange.Start);
    }

    private static IEnumerable<TaskContainerReturn> GetDynamicTasksForTimeRange(IEnumerable<DynamicTask> dynamicTasks, TimeRange timeRange)
    {
        var dynamicTaskContainers = dynamicTasks
            .Where(dt => dt.MaxTimeToFinish.TotalMinutes > 0)
            .Select(dt => new DynamicTaskContainer(dt))
            .ToList();

        var dynamicTaskContainerTimeline = GetDynamicTasksForTimeRangeRecursive(dynamicTaskContainers, timeRange);
        var dynamicTaskTimeline = dynamicTaskContainerTimeline.Select(dt => new TaskContainerReturn
        {
            Task = dt.Task,
            IsFixed = false,
            TimeRange = dt.TimeRange
        });

        return dynamicTaskTimeline;
    }

    private static IEnumerable<DynamicTaskContainer> GetDynamicTasksForTimeRangeRecursive(IEnumerable<DynamicTaskContainer> dynamicTasks, TimeRange timeRange)
    {
        var timeToFinish = timeRange.End - timeRange.Start;
        dynamicTasks = dynamicTasks.Where(dt => dt.Task.MinTimeToFinish <= timeToFinish).ToList(); // ensure that the task can be finished in the given time range
        if (!dynamicTasks.Any())
            return [];

        var possibleTimelines = new List<(IEnumerable<DynamicTaskContainer> DynamicTasks, float Score)>();

        //limit count of iterations (for performance)
        var takeCount = timeToFinish switch
        {
            { TotalHours: < 2 } => 4,
            { TotalHours: < 4 } => 3,
            { TotalHours: < 6 } => 2,
            { TotalHours: < 8 } => 1,
            _ => 1
        };

        var weightedDynamicTasks = dynamicTasks.Select(dt => (dt, 1 / (float)(dt.CountOfUses + dt.Task.Priority + 1))).ToList();
        var chosenDynamicTasks = RandomValuesHelper.GetRandomEntries(weightedDynamicTasks, takeCount).ToList(); // shuffle the tasks and take only several of them

        foreach (var dynamicTask in chosenDynamicTasks)
        {
            TimeSpan taskTime;
            if (dynamicTask.Task.OptimalTimeToFinish != null && dynamicTask.Task.OptimalTimeToFinish.Value != TimeSpan.Zero)
                taskTime = dynamicTask.Task.OptimalTimeToFinish.Value;
            else
            {
                var minMinutes = Convert.ToInt32(Math.Round(dynamicTask.Task.MinTimeToFinish.TotalMinutes));
                var maxMinutes = Convert.ToInt32(Math.Round(dynamicTask.Task.MaxTimeToFinish.TotalMinutes));
                // Random.Next(min, max) requires max > min; guard equal/inverted bounds after rounding.
                var chosenMinutes = minMinutes >= maxMinutes ? minMinutes : Random.Shared.Next(minMinutes, maxMinutes);
                taskTime = TimeSpan.FromMinutes(chosenMinutes);
            }

            if (taskTime > timeToFinish)
                taskTime = timeToFinish;

            var chosenDynamicTask = dynamicTask with
            {
                CountOfUses = dynamicTask.CountOfUses + 1,
                TimeRange = new TimeRange(timeRange.Start, timeRange.Start + taskTime)
            };

            var newTimeRange = new TimeRange(timeRange.Start + taskTime + DaytimeConstants.TimeBacklashBetweenTasks, timeRange.End);
            var newTimeToFinish = newTimeRange.End - newTimeRange.Start;

            var dynamicTasksCopy = dynamicTasks
                .Where(dt => dt.Task.MinTimeToFinish <= newTimeToFinish)
                .Where(dt => dt.Task.Id != dynamicTask.Task.Id).ToList();

            if (chosenDynamicTask.Task.MinTimeToFinish <= newTimeToFinish)
                dynamicTasksCopy.Add(chosenDynamicTask);

            var possibleTaskTimeline = new List<DynamicTaskContainer>();
            if (dynamicTasksCopy.Count != 0)
                possibleTaskTimeline = GetDynamicTasksForTimeRangeRecursive(dynamicTasksCopy, newTimeRange).ToList();

            possibleTaskTimeline.Add(chosenDynamicTask);

            var distinctTasks = possibleTaskTimeline.DistinctBy(dt => dt.Task.Id).ToList();
            var tasksCountOfUses = possibleTaskTimeline.Sum(dt => dt.CountOfUses);
            var prioritySum = distinctTasks.Sum(dt => dt.Task.Priority);
            var score = (float)(tasksCountOfUses + prioritySum) / distinctTasks.Count;

            var maxTimeRangeEnd = possibleTaskTimeline.Max(tt => tt.TimeRange.End);
            score += (float)(timeRange.End - maxTimeRangeEnd).TotalMinutes; // penalty for not using the whole time range

            // Lower score is better; invert into a weight, guarding against division by zero (Infinity).
            var weight = score <= 0 ? float.MaxValue : 1 / score;
            possibleTimelines.Add((possibleTaskTimeline, weight));
        }
        if(possibleTimelines.Count == 0)
            return [];

        var randomDynamicTask = RandomValuesHelper.GetRandomEntries(possibleTimelines, 1).First();
        return randomDynamicTask;
    }
}
