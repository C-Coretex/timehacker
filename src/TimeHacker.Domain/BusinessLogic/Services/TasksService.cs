using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        private readonly string userId;
        public TasksService(IDynamicTasksServiceQuery dynamicTasksServiceQuery, IFixedTasksServiceQuery fixedTasksServiceQuery, IHttpContextAccessor httpContextAccessor)
        {
            _dynamicTasksServiceQuery = dynamicTasksServiceQuery;
            _fixedTasksServiceQuery = fixedTasksServiceQuery;
            userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<TasksForDayReturn> GetTasksForDay(DateTime date)
        {
            var returnData = new TasksForDayReturn();

            var dynamicTasks = await _dynamicTasksServiceQuery.GetAllByUserId(userId)
                                                            .ToListAsync();

            var fixedTasks = _fixedTasksServiceQuery.GetAllByUserId(userId)
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
            foreach(var takenTimeRange in returnData.TasksTimeline.Select(tt => tt.TimeRange))
            {
                timeRange = new TimeRange(startTimeSpan, takenTimeRange.Start - Constants.DefaultConstants.TimeBacklashBetweenTasks);

                if (timeRange.Start < timeRange.End)
                {
                    var tasks = DynamicTasksHelpers.GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
                    returnData.TasksTimeline.AddRange(tasks);
                }

                startTimeSpan = takenTimeRange.End + Constants.DefaultConstants.TimeBacklashBetweenTasks;
            }

            timeRange = new TimeRange(startTimeSpan, Constants.DefaultConstants.EndOfDay);
            if(timeRange.Start < timeRange.End)
            {
                var tasks = DynamicTasksHelpers.GetDynamicTasksForTimeRange(dynamicTasks, timeRange);
                returnData.TasksTimeline.AddRange(tasks);
            }

            returnData.TasksTimeline = returnData.TasksTimeline.OrderBy(t => t.TimeRange.Start).ToList();
            return returnData;
        }
    }
}
