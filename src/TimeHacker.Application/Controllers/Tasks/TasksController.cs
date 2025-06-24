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

        private readonly IMapper _mapper;

        public TasksController(ITaskService taskService, IScheduledTaskService scheduledTaskService, IScheduleEntityService scheduleEntityService, IMapper mapper)
        {
            _taskService = taskService;
            _scheduledTaskService = scheduledTaskService;
            _scheduleEntityService = scheduleEntityService;

            _mapper = mapper;
        }

        [HttpGet("GetTasksForDay")]
        public async Task<IActionResult> GetTasksForDay(string date)
        {
            var dateParsed = DateOnly.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            var data = await _taskService.GetTasksForDay(dateParsed);

            return Ok(data);
        }

        [HttpGet("GetTasksForDays")]
        public IActionResult GetTasksForDays([FromBody] ICollection<DateOnly> dates)
        {
            var data = _taskService.GetTasksForDays(dates);

            return Ok(data);
        }

        [HttpPost("RefreshTasksForDays")]
        public IActionResult RefreshTasksForDays([FromBody] ICollection<DateOnly> dates)
        {
            var data = _taskService.RefreshTasksForDays(dates);

            return Ok(data);
        }

        [HttpGet("GetScheduledTaskById/{id}")]
        public async Task<IActionResult> GetScheduledTaskById(ulong id)
        {
            var data = _mapper.Map<ScheduledTaskReturnModel>(await _scheduledTaskService.GetBy(id));

            return Ok(data);
        }

        [HttpPost("PostNewScheduleForTask")]
        public async Task<IActionResult> PostNewScheduleForTask([FromBody] InputScheduleEntityModel inputScheduleEntityModel)
        {
            var data = _mapper.Map<ScheduleEntityReturnModel>(await _scheduleEntityService.Save(inputScheduleEntityModel));

            return Ok(data);
        }
    }
}
