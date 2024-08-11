﻿using TimeHacker.Domain.Contracts.Constants;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;
using TimeHacker.Domain.Contracts.Models.ReturnModels;
using TimeHacker.Helpers.Domain.Helpers;

namespace TimeHacker.Domain.Processors
{
    public class TaskTimelineProcessor
    {
        public TaskTimelineProcessor() { }

        public TasksForDayReturn GetTasksForDay(IEnumerable<FixedTask> fixedTasks, IEnumerable<DynamicTask> dynamicTasks, DateTime date)
        {
            var returnData = new TasksForDayReturn()
            {
                Date = new DateOnly(date.Year, date.Month, date.Day)
            };

            var fixedTasksTimeline = GetFixedTasksTimeline(fixedTasks);
            returnData.TasksTimeline.AddRange(fixedTasksTimeline);

            var timeRanges = returnData.TasksTimeline.Select(tt => tt.TimeRange);
            var dynamicTasksTimeline = GetDynamicTasksTimeline(dynamicTasks.ToList(), timeRanges);
            returnData.TasksTimeline.AddRange(dynamicTasksTimeline);

            returnData.TasksTimeline = returnData.TasksTimeline.OrderBy(t => t.TimeRange.Start).ToList();
            return returnData;
        }

        #region Protected

        protected IEnumerable<TaskContainerReturn> GetFixedTasksTimeline(IEnumerable<FixedTask> fixedTasks)
        {
            return fixedTasks.Select(fixedTask => new TaskContainerReturn()
            {
                Task = new TaskBase(fixedTask),
                IsFixed = true,
                TimeRange = new TimeRange(fixedTask.StartTimestamp.TimeOfDay, fixedTask.EndTimestamp.TimeOfDay)
            });
        }

        protected IEnumerable<TaskContainerReturn> GetDynamicTasksTimeline(IList<DynamicTask> dynamicTasks, IEnumerable<TimeRange> timeRanges)
        {
            var startTimeSpan = DefaultConstants.StartOfDay;
            TimeRange timeRange;
            var dynamicTasksTimeline = new List<TaskContainerReturn>();
            foreach (var takenTimeRange in timeRanges)
            {
                timeRange = new TimeRange(startTimeSpan, takenTimeRange.Start - DefaultConstants.TimeBacklashBetweenTasks);

                if (timeRange.Start < timeRange.End)
                {
                    var tasks = GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
                    dynamicTasksTimeline.AddRange(tasks);
                }

                startTimeSpan = takenTimeRange.End + DefaultConstants.TimeBacklashBetweenTasks;
            }

            timeRange = new TimeRange(startTimeSpan, DefaultConstants.EndOfDay);
            if (timeRange.Start < timeRange.End)
            {
                var tasks = GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
                dynamicTasksTimeline.AddRange(tasks);
            }

            return dynamicTasksTimeline;
        }

        protected static IEnumerable<TaskContainerReturn> GetDynamicTasksForTimeRange(IEnumerable<DynamicTask> dynamicTasks, TimeRange timeRange)
        {
            var dynamicTaskContainers = dynamicTasks
                .Where(dt => dt.MaxTimeToFinish.TotalMinutes > 0)
                .Select(dt => new DynamicTaskContainer(dt))
                .ToList();

            var dynamicTaskContainerTimeline = GetDynamicTasksForTimeRangeRecursive(dynamicTaskContainers, timeRange);
            var dynamicTaskTimeline = dynamicTaskContainerTimeline.Select(dt => new TaskContainerReturn
            {
                Task = new TaskBase(dt.Task),
                IsFixed = false,
                TimeRange = dt.TimeRange
            });

            return dynamicTaskTimeline;
        }

        protected static IEnumerable<DynamicTaskContainer> GetDynamicTasksForTimeRangeRecursive(IEnumerable<DynamicTaskContainer> dynamicTasks, TimeRange timeRange)
        {
            var timeToFinish = timeRange.End - timeRange.Start;
            dynamicTasks = dynamicTasks.Where(dt => dt.Task.MinTimeToFinish <= timeToFinish).ToList(); // ensure that the task can be finished in the given time range
            if (!dynamicTasks.Any())
                return [];

            var possibleTimelines = new List<(IEnumerable<DynamicTaskContainer> DynamicTasks, float Score)>();

            //limit count of iterations (for performance)
            var takeCount = timeToFinish switch
            {
                var t when t.TotalHours < 2 => 6,
                var t when t.TotalHours < 4 => 5,
                var t when t.TotalHours < 6 => 3,
                var t when t.TotalHours < 8 => 2,
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
                    var chosenMinutes = Random.Shared.Next(minMinutes, maxMinutes);
                    taskTime = TimeSpan.FromMinutes(chosenMinutes);
                }

                if (taskTime > timeToFinish)
                    taskTime = timeToFinish;

                var chosenDynamicTask = dynamicTask with
                {
                    CountOfUses = dynamicTask.CountOfUses + 1,
                    TimeRange = new TimeRange(timeRange.Start, timeRange.Start + taskTime)
                };

                var newTimeRange = new TimeRange(timeRange.Start + taskTime + DefaultConstants.TimeBacklashBetweenTasks, timeRange.End);
                var newTimeToFinish = newTimeRange.End - newTimeRange.Start;

                var dynamicTasksCopy = dynamicTasks
                    .Where(dt => dt.Task.MinTimeToFinish <= newTimeToFinish)
                    .Where(dt => dt.Task.Id != dynamicTask.Task.Id).ToList();

                if (chosenDynamicTask.Task.MinTimeToFinish <= newTimeToFinish)
                    dynamicTasksCopy.Add(chosenDynamicTask);

                var possibleTaskTimeline = new List<DynamicTaskContainer>();
                if (dynamicTasksCopy.Any())
                    possibleTaskTimeline = GetDynamicTasksForTimeRangeRecursive(dynamicTasksCopy, newTimeRange).ToList();

                possibleTaskTimeline.Add(chosenDynamicTask);

                var distinctTasks = possibleTaskTimeline.DistinctBy(dt => dt.Task.Id).ToList();
                var tasksCountOfUses = possibleTaskTimeline.Sum(dt => dt.CountOfUses);
                var prioritySum = distinctTasks.Sum(dt => dt.Task.Priority);
                var score = (float)((tasksCountOfUses + prioritySum) / distinctTasks.Count);

                var maxTimeRangeEnd = possibleTaskTimeline.Max(tt => tt.TimeRange.End);
                score += (timeRange.End - maxTimeRangeEnd).Minutes; // penalty for not using the whole time range

                possibleTimelines.Add((possibleTaskTimeline, 1 / score));
            }

            var randomDynamicTask = RandomValuesHelper.GetRandomEntries(possibleTimelines, 1).First();
            return randomDynamicTask;
        }

        #endregion
    }
}
