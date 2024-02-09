using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
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

        public async Task<IEnumerable<TasksForDayReturn>> GetTasksForDay(DateTime date)
        {
            var dynamicTasks = _dynamicTasksServiceQuery.GetAllByUserId(userId).ToListAsync();
            var fixedTasks = _fixedTasksServiceQuery.GetAllByUserId(userId).Where(ft => ft.StartTimestamp.Date == date)
                                                                           .ToListAsync();
            return Enumerable.Empty<TasksForDayReturn>();
        }
    }
}
