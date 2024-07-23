using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TimeHacker.Domain.Contracts.IServices.Tasks;

namespace TimeHacker.Application.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/Tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;
        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet("GetTasksForDay")]
        public async Task<IActionResult> GetTasksForDay(string date)
        {
            try
            {
                var dateParsed = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                var data = await _taskService.GetTasksForDay(dateParsed);

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting tasks for day");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetTasksForDays")]
        public IActionResult GetTasksForDays([FromBody] IEnumerable<DateTime> dates)
        {
            try
            {
                var data = _taskService.GetTasksForDays(dates);

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
