using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using AutoMapper;
using TimeHacker.Application.Models.Return.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Application.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/Tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IScheduledTaskService _scheduledTaskService;
        private readonly IScheduleEntityService _scheduleEntityService;

        private readonly ILogger<TasksController> _logger;
        private readonly IMapper _mapper;

        public TasksController(ITaskService taskService, IScheduledTaskService scheduledTaskService, IScheduleEntityService scheduleEntityService, ILogger<TasksController> logger, IMapper mapper)
        {
            _taskService = taskService;
            _scheduledTaskService = scheduledTaskService;
            _scheduleEntityService = scheduleEntityService;

            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetTasksForDay")]
        public async Task<IActionResult> GetTasksForDay(string date)
        {
            try
            {
                var dateParsed = DateOnly.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
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
        public IActionResult GetTasksForDays([FromBody] IEnumerable<DateOnly> dates)
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

        [HttpPost("RefreshTasksForDays")]
        public IActionResult RefreshTasksForDays([FromBody] IEnumerable<DateOnly> dates)
        {
            try
            {
                var data = _taskService.RefreshTasksForDays(dates);

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while refreshing tasks for days");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetScheduledTaskById/{id}")]
        public async Task<IActionResult> GetScheduledTaskById(ulong id)
        {
            try
            {
                var data = _mapper.Map<ScheduledTaskReturnModel>(await _scheduledTaskService.GetBy(id));

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting fixed task by id");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("PostNewScheduleForTask")]
        public async Task<IActionResult> PostNewScheduleForTask([FromBody] InputScheduleEntityModel inputScheduleEntityModel)
        {
            try
            {
                var data = _mapper.Map<ScheduleEntityReturnModel>(await _scheduleEntityService.Save(inputScheduleEntityModel));

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while posting new Schedule for a task");
                return BadRequest(e.Message);
            }
        }
    }
}
