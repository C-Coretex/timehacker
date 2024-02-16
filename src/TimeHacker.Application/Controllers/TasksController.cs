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
        private readonly ILogger<TasksController> _logger;
        public TasksController(TasksService tasksService, ILogger<TasksController> logger)
        {
            _tasksService = tasksService;
            _logger = logger;
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
                _logger.LogError(e, "Error while getting tasks for day");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetTasksForDays")]
        public async Task<IActionResult> GetTasksForDays([FromBody] IEnumerable<DateTime> dates)
        {
            try
            {
                var data = await _tasksService.GetTasksForDays(dates);

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting tasks for days");
                return BadRequest(e.Message);
            }
        }
    }
}
