using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Tasks;

namespace TimeHacker.Application.Pages
{
    public class TasksAndEventsModel : PageModel
    {
        private readonly IDynamicTasksServiceQuery _dynamicTasksServiceQuery;
        private readonly IFixedTasksServiceQuery _fixedTasksServiceQuery;
        public IEnumerable<DynamicTask> DynamicTasks { get; set; } = Enumerable.Empty<DynamicTask>();
        public IEnumerable<FixedTask> FixedTasks { get; set; } = Enumerable.Empty<FixedTask>();
        public TasksAndEventsModel(IDynamicTasksServiceQuery dynamicTasksServiceQuery, IFixedTasksServiceQuery fixedTasksServiceQuery)
        {
            _dynamicTasksServiceQuery = dynamicTasksServiceQuery;
            _fixedTasksServiceQuery = fixedTasksServiceQuery;
        }

        public void OnGet()
        {
            //TODO: get by user id
            DynamicTasks = _dynamicTasksServiceQuery.GetAll().AsNoTracking().ToList();
            FixedTasks = _fixedTasksServiceQuery.GetAll().AsNoTracking().ToList();
        }
    }
}
