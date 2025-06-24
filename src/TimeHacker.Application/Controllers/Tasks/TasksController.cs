using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TimeHacker.Application.Models.Return.Categories;
using TimeHacker.Application.Models.Return.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.ReturnModels;

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

        [ProducesResponseType(typeof(TasksForDayReturn), StatusCodes.Status200OK)]
        [HttpGet("GetTasksForDay")]
        public async Task<Ok<TasksForDayReturn>> GetTasksForDay(string date)
        {
            var dateParsed = DateOnly.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            var data = await _taskService.GetTasksForDay(dateParsed);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(IAsyncEnumerable<TasksForDayReturn>), StatusCodes.Status200OK)]
        [HttpGet("GetTasksForDays")]
        public Ok<IAsyncEnumerable<TasksForDayReturn>> GetTasksForDays([FromBody] ICollection<DateOnly> dates)
        {
            var data = _taskService.GetTasksForDays(dates);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(IAsyncEnumerable<TasksForDayReturn>), StatusCodes.Status200OK)]
        [HttpPost("RefreshTasksForDays")]
        public Ok<IAsyncEnumerable<TasksForDayReturn>> RefreshTasksForDays([FromBody] ICollection<DateOnly> dates)
        {
            var data = _taskService.RefreshTasksForDays(dates);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(ScheduledTaskReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetScheduledTaskById/{id}")]
        public async Task<Results<Ok<ScheduledTaskReturnModel>, NotFound>> GetScheduledTaskById(ulong id)
        {
            var entity = await _scheduledTaskService.GetBy(id);
            if(entity == null)
                return TypedResults.NotFound();

            var data = _mapper.Map<ScheduledTaskReturnModel>(entity);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(ScheduleEntityReturnModel), StatusCodes.Status200OK)]
        [HttpPost("PostNewScheduleForTask")]
        public async Task<Ok<ScheduleEntityReturnModel>> PostNewScheduleForTask([FromBody] InputScheduleEntityModel inputScheduleEntityModel)
        {
            var entity = await _scheduleEntityService.Save(inputScheduleEntityModel);

            var data = _mapper.Map<ScheduleEntityReturnModel>(entity);

            return TypedResults.Ok(data);
        }
    }
}
