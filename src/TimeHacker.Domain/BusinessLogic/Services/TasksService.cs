using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
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
            var fixedTasks = await _fixedTasksServiceQuery.GetAllByUserId(userId)
                                                            .Where(ft => ft.StartTimestamp.Date == date)
                                                            .OrderBy(ft => ft.StartTimestamp)
                                                            .ToListAsync();

            var timeRanges = new List<TimeRange>();

            foreach(var fixedTask in fixedTasks)
            {
                var taskContainer = new TaskContainerReturn
                {
                    Task = fixedTask,
                    TimeRange = new TimeRange(fixedTask.StartTimestamp.TimeOfDay, fixedTask.EndTimestamp.TimeOfDay)
                };

                returnData.TasksTimeline.Add(taskContainer);
            }



            return returnData;
        }
    }
}
