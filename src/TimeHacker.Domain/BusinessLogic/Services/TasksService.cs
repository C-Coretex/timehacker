using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeHacker.Domain.Abstractions.Interfaces.Helpers;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.BusinessLogic.Helpers;
using TimeHacker.Domain.Models.BusinessLogicModels;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Domain.Models.ReturnModels;

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
            var returnData = new TasksForDayReturn();

            var dynamicTasks = await _dynamicTasksServiceQuery.GetAllByUserId(_userAccessor.UserId)
                                                            .ToListAsync();

            var fixedTasks = _fixedTasksServiceQuery.GetAllByUserId(_userAccessor.UserId)
                                                            .Where(ft => ft.StartTimestamp.Date == date.Date)
                                                            .OrderBy(ft => ft.StartTimestamp)
                                                            .AsAsyncEnumerable();

            var timeRanges = new List<TimeRange>();

            await foreach(var fixedTask in fixedTasks)
            {
                var taskContainer = new TaskContainerReturn
                {
                    Task = fixedTask,
                    IsFixed = true,
                    TimeRange = new TimeRange(fixedTask.StartTimestamp.TimeOfDay, fixedTask.EndTimestamp.TimeOfDay)
                };

                returnData.TasksTimeline.Add(taskContainer);
            }

            var startTimeSpan = Constants.DefaultConstants.StartOfDay;
            TimeRange timeRange;
            var dynamicTasksTimeline = new List<TaskContainerReturn>();
            foreach(var takenTimeRange in returnData.TasksTimeline.Select(tt => tt.TimeRange))
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
            if(timeRange.Start < timeRange.End)
            {
                var tasks = DynamicTasksHelpers.GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
                dynamicTasksTimeline.AddRange(tasks);
            }

            returnData.TasksTimeline.AddRange(dynamicTasksTimeline);

            returnData.TasksTimeline = returnData.TasksTimeline.OrderBy(t => t.TimeRange.Start).ToList();
            return returnData;
        }
    }
}
