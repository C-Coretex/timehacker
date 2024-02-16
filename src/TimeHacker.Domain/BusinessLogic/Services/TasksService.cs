using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using TimeHacker.Domain.Abstractions.Interfaces.Helpers;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.BusinessLogic.Helpers;
using TimeHacker.Domain.Models.BusinessLogicModels;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Domain.Models.ReturnModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeHacker.Domain.BusinessLogic.Services
{
    public class TasksService
    {
        private readonly IDynamicTasksServiceQuery _dynamicTasksServiceQuery;
        private readonly IFixedTasksServiceQuery _fixedTasksServiceQuery;
        private readonly IUserAccessor _userAccessor;
        public TasksService(IDynamicTasksServiceQuery dynamicTasksServiceQuery, IFixedTasksServiceQuery fixedTasksServiceQuery, IUserAccessor userAccessor)
        {
            _dynamicTasksServiceQuery = dynamicTasksServiceQuery;
            _fixedTasksServiceQuery = fixedTasksServiceQuery;

            if(!userAccessor.IsUserValid)
                throw new ArgumentNullException(nameof(userAccessor.UserId));

            _userAccessor = userAccessor;
        }

        public async Task<TasksForDayReturn> GetTasksForDay(DateTime date)
        {
            var returnData = new TasksForDayReturn()
            { 
                Date = new DateOnly(date.Year, date.Month, date.Day)
            };

            var fixedTasks = _fixedTasksServiceQuery.GetAllByUserId(_userAccessor.UserId)
                                                .Where(ft => ft.StartTimestamp.Date == date.Date)
                                                .OrderBy(ft => ft.StartTimestamp)
                                                .AsAsyncEnumerable();

            var dynamicTasks = await _dynamicTasksServiceQuery.GetAllByUserId(_userAccessor.UserId)
                                                            .ToListAsync();

            var fixedTasksTimeline = GetFixedTasksAsync(fixedTasks);
            await foreach (var fixedTask in fixedTasksTimeline)
                returnData.TasksTimeline.Add(fixedTask);

            var timeRanges = returnData.TasksTimeline.Select(tt => tt.TimeRange);
            var dynamicTasksTimeline = GetDynamicTasks(dynamicTasks, timeRanges);
            returnData.TasksTimeline.AddRange(dynamicTasksTimeline);

            returnData.TasksTimeline = returnData.TasksTimeline.OrderBy(t => t.TimeRange.Start).ToList();
            return returnData;
        }

        public async Task<IEnumerable<TasksForDayReturn>> GetTasksForDays(IEnumerable<DateTime> dates)
        {
            var fixedTasks = await _fixedTasksServiceQuery.GetAllByUserId(_userAccessor.UserId)
                                                            .Where(ft => dates.Any(d => d.Date == ft.StartTimestamp.Date))
                                                            .OrderBy(ft => ft.StartTimestamp)
                                                            .ToListAsync();

            var dynamicTasks = await _dynamicTasksServiceQuery.GetAllByUserId(_userAccessor.UserId)
                                                            .ToListAsync();

            var tasksForDays = new List<TasksForDayReturn>();

            foreach(var date in dates)
            {
                var tasksForDayReturn = new TasksForDayReturn()
                {
                    Date = new DateOnly(date.Year, date.Month, date.Day)
                };

                var fixedTasksForDay = fixedTasks.Where(ft => ft.StartTimestamp.Date == date.Date);
                var fixedTasksTimeline = GetFixedTasks(fixedTasksForDay);
                tasksForDayReturn.TasksTimeline.AddRange(fixedTasksTimeline);

                var timeRanges = tasksForDayReturn.TasksTimeline.Select(tt => tt.TimeRange);

                var dynamicTasksTimeline = GetDynamicTasks(dynamicTasks, timeRanges);
                tasksForDayReturn.TasksTimeline.AddRange(dynamicTasksTimeline);

                tasksForDayReturn.TasksTimeline = tasksForDayReturn.TasksTimeline.OrderBy(t => t.TimeRange.Start).ToList();
                tasksForDays.Add(tasksForDayReturn);
            }

            return tasksForDays;
        }

        protected async IAsyncEnumerable<TaskContainerReturn> GetFixedTasksAsync(IAsyncEnumerable<FixedTask> fixedTasks)
        {
            await foreach (var fixedTask in fixedTasks)
            {
                var taskContainer = new TaskContainerReturn
                {
                    Task = fixedTask,
                    IsFixed = true,
                    TimeRange = new TimeRange(fixedTask.StartTimestamp.TimeOfDay, fixedTask.EndTimestamp.TimeOfDay)
                };

                yield return taskContainer;
            }
        }

        protected IEnumerable<TaskContainerReturn> GetFixedTasks(IEnumerable<FixedTask> fixedTasks)
        {
            foreach (var fixedTask in fixedTasks)
            {
                var taskContainer = new TaskContainerReturn
                {
                    Task = fixedTask,
                    IsFixed = true,
                    TimeRange = new TimeRange(fixedTask.StartTimestamp.TimeOfDay, fixedTask.EndTimestamp.TimeOfDay)
                };

                yield return taskContainer;
            }
        }

        protected IEnumerable<TaskContainerReturn> GetDynamicTasks(IEnumerable<DynamicTask> dynamicTasks, IEnumerable<TimeRange> timeRanges)
        {
            var startTimeSpan = Constants.DefaultConstants.StartOfDay;
            TimeRange timeRange;
            var dynamicTasksTimeline = new List<TaskContainerReturn>();
            foreach (var takenTimeRange in timeRanges)
            {
                timeRange = new TimeRange(startTimeSpan, takenTimeRange.Start - Constants.DefaultConstants.TimeBacklashBetweenTasks);

                if (timeRange.Start < timeRange.End)
                {
                    var tasks = DynamicTasksHelpers.GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
                    dynamicTasksTimeline.AddRange(tasks);
                }

                startTimeSpan = takenTimeRange.End + Constants.DefaultConstants.TimeBacklashBetweenTasks;
            }

            timeRange = new TimeRange(startTimeSpan, Constants.DefaultConstants.EndOfDay);
            if (timeRange.Start < timeRange.End)
            {
                var tasks = DynamicTasksHelpers.GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
                dynamicTasksTimeline.AddRange(tasks);
            }

            return dynamicTasksTimeline;
        }
    }
}
