using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

namespace TimeHacker.Api.Controllers.Tasks;

[Authorize]
[ApiController]
[Route("/api/Tasks")]
public class TasksController(ITaskAppService taskService)
    : ControllerBase
{
    [ProducesResponseType(typeof(TasksForDayReturn), StatusCodes.Status200OK)]
    [HttpGet("GetTasksForDay")]
    public async Task<Ok<TasksForDayDto>> GetTasksForDay(string date, CancellationToken cancellationToken = default)
    {
        var dateParsed = DateOnly.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        var data = await taskService.GetTasksForDay(dateParsed, cancellationToken);

        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(IAsyncEnumerable<TasksForDayReturn>), StatusCodes.Status200OK)]
    [HttpGet("GetTasksForDays")]
    public Ok<IAsyncEnumerable<TasksForDayDto>> GetTasksForDays([FromBody] ICollection<DateOnly> dates, CancellationToken cancellationToken = default)
    {
        var data = taskService.GetTasksForDays(dates, cancellationToken);

        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(IAsyncEnumerable<TasksForDayReturn>), StatusCodes.Status200OK)]
    [HttpPost("RefreshTasksForDays")]
    public Ok<IAsyncEnumerable<TasksForDayDto>> RefreshTasksForDays([FromBody] ICollection<DateOnly> dates, CancellationToken cancellationToken = default)
    {
        var data = taskService.RefreshTasksForDays(dates, cancellationToken);

        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(ScheduledTaskReturnModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetScheduledTaskById/{id}")]
    public async Task<Results<Ok<ScheduledTaskReturnModel>, NotFound>> GetScheduledTaskById(
        Guid id,
        [FromServices] IScheduledTaskAppService scheduledTaskAppService,
        CancellationToken cancellationToken = default)
    {
        var entity = await scheduledTaskAppService.GetBy(id, cancellationToken);
        if(entity == null)
            return TypedResults.NotFound();

        var data = ScheduledTaskReturnModel.Create(entity);

        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(ScheduleEntityReturnModel), StatusCodes.Status200OK)]
    [HttpPost("PostNewScheduleForTask")]
    public async Task<Ok<ScheduleEntityReturnModel>> PostNewScheduleForTask(
        [FromBody] InputScheduleEntityModel inputScheduleEntityModel,
        [FromServices] IScheduleEntityAppService scheduleEntityAppService,
        CancellationToken cancellationToken = default)
    {
        var entity = await scheduleEntityAppService.Save(inputScheduleEntityModel.CreateDto(), cancellationToken);
        var data = ScheduleEntityReturnModel.Create(entity);

        return TypedResults.Ok(data);
    }
}
