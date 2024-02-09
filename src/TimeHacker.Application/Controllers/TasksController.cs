using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TimeHacker.Domain.BusinessLogic.Services;

namespace TimeHacker.Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TasksService _tasksService;
        public TasksController(TasksService tasksService)
        {
            _tasksService = tasksService;
        }

        [HttpGet("GetTasksForDay")]
        public async Task<IActionResult> GetTasksForDay(string date)
        {
            try
            {
                var dateParsed = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                var data = await _tasksService.GetTasksForDay(dateParsed);

                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /*[HttpGet]
        public Task<IActionResult> GetTasks()
        {
            return _tasksService.GetTasks();
        }*/
    }
}
