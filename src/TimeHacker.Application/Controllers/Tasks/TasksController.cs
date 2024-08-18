﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Services.Tasks;

namespace TimeHacker.Application.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/Tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IScheduleSnapshotService _scheduleSnapshotService;
        private readonly IScheduledTaskService _scheduledTaskService;
        private readonly ILogger<TasksController> _logger;
        public TasksController(ITaskService taskService, IScheduleSnapshotService scheduleSnapshotService, IScheduledTaskService scheduledTaskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _scheduleSnapshotService = scheduleSnapshotService;
            _scheduledTaskService = scheduledTaskService;

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

        [HttpPost("RefreshTasksForDays")]
        public IActionResult RefreshTasksForDays([FromBody] IEnumerable<DateTime> dates)
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
        public async Task<IActionResult> GetScheduledTaskById(Guid id)
        {
            try
            {
                var data = await _scheduledTaskService.GetBy(id);

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting fixed task by id");
                return BadRequest(e.Message);
            }
        }
    }
}
