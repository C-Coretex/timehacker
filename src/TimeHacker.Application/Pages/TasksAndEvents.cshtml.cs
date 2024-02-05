using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Tasks;

namespace TimeHacker.Application.Pages
{
    public class TasksAndEventsModel : PageModel
    {
        private readonly IDynamicTasksServiceQuery _dynamicTasksServiceQuery;
        private readonly IFixedTasksServiceQuery _fixedTasksServiceQuery;
        private readonly string _userId;
        public IEnumerable<DynamicTask> DynamicTasks { get; set; } = Enumerable.Empty<DynamicTask>();
        public IEnumerable<FixedTask> FixedTasks { get; set; } = Enumerable.Empty<FixedTask>();
        public TasksAndEventsModel(IDynamicTasksServiceQuery dynamicTasksServiceQuery, IFixedTasksServiceQuery fixedTasksServiceQuery, IHttpContextAccessor httpContextAccessor)
        {
            _dynamicTasksServiceQuery = dynamicTasksServiceQuery;
            _fixedTasksServiceQuery = fixedTasksServiceQuery;
            var user = httpContextAccessor.HttpContext?.User;
            _userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not found");
        }

        public void OnGet()
        {
            DynamicTasks = _dynamicTasksServiceQuery.GetAllByUserId(_userId).AsNoTracking();
            FixedTasks = _fixedTasksServiceQuery.GetAllByUserId(_userId).AsNoTracking();
        }
    }
}
