using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeHacker.Application.BusinessLogic.Services;

namespace TimeHacker.Application.Controllers
{
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TasksService _tasksService;
        public TasksController(TasksService tasksService)
        {
            _tasksService = tasksService;
        }

        /*[HttpGet]
        public Task<IActionResult> GetTasks()
        {
            return _tasksService.GetTasks();
        }*/
    }
}
