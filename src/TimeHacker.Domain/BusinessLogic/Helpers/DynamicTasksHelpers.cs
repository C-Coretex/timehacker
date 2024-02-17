using System.Security.Cryptography;
using TimeHacker.Domain.Models.BusinessLogicModels;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Domain.Models.ReturnModels;

namespace TimeHacker.Domain.BusinessLogic.Helpers
{
    public class DynamicTasksHelpers
    {
        public static IEnumerable<TaskContainerReturn> GetDynamicTasksForTimeRange(IEnumerable<DynamicTask> dynamicTasks, TimeRange timeRange)
        {
            var dynamicTaskContainers = dynamicTasks
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

            var chosenDynamicTasks = dynamicTasks.OrderBy(dt => new Guid()).Take(takeCount); // shuffle the tasks and take only 5 of them

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

                var newTimeRange = new TimeRange(timeRange.Start + taskTime + Constants.DefaultConstants.TimeBacklashBetweenTasks, timeRange.End);
                var newTimeToFinish = newTimeRange.End - newTimeRange.Start;

                var dynamicTasksCopy = dynamicTasks
                    .Where(dt => dt.Task.MinTimeToFinish <= newTimeToFinish)    
                    .Where(dt => dt.Task.Id != dynamicTask.Task.Id).ToList();

                if(chosenDynamicTask.Task.MinTimeToFinish <= newTimeToFinish)
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

            var totalScore = possibleTimelines.Sum(pt => pt.Score);
            var randomValue = totalScore * (float)Random.Shared.NextDouble();
            possibleTimelines = possibleTimelines.OrderBy(pt => pt.Score).ToList();

            foreach(var possibleTimeline in possibleTimelines)
            {
                randomValue -= possibleTimeline.Score;
                if(randomValue <= 0)
                    return possibleTimeline.DynamicTasks;
            }

            return [];
        }
    }
}
