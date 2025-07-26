using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using TimeHacker.Api.Models.Return.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.IServices;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Models.ReturnModels;

namespace TimeHacker.Api.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/Tasks")]
    public class TasksController(ITaskAppService taskService)
        : ControllerBase
    {
        [ProducesResponseType(typeof(TasksForDayReturn), StatusCodes.Status200OK)]
        [HttpGet("GetTasksForDay")]
        public async Task<Ok<TasksForDayReturn>> GetTasksForDay(string date)
        {
            var dateParsed = DateOnly.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            var data = await taskService.GetTasksForDay(dateParsed);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(IAsyncEnumerable<TasksForDayReturn>), StatusCodes.Status200OK)]
        [HttpGet("GetTasksForDays")]
        public Ok<IAsyncEnumerable<TasksForDayReturn>> GetTasksForDays([FromBody] ICollection<DateOnly> dates)
        {
            var data = taskService.GetTasksForDays(dates);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(IAsyncEnumerable<TasksForDayReturn>), StatusCodes.Status200OK)]
        [HttpPost("RefreshTasksForDays")]
        public Ok<IAsyncEnumerable<TasksForDayReturn>> RefreshTasksForDays([FromBody] ICollection<DateOnly> dates)
        {
            var data = taskService.RefreshTasksForDays(dates);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(ScheduledTaskReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetScheduledTaskById/{id}")]
        public async Task<Results<Ok<ScheduledTaskReturnModel>, NotFound>> GetScheduledTaskById(
            Guid id, 
            [FromServices] IScheduledTaskAppService scheduledTaskAppService)
        {
            var entity = await scheduledTaskAppService.GetBy(id);
            if(entity == null)
                return TypedResults.NotFound();

            var data = ScheduledTaskReturnModel.Create(entity);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(ScheduleEntityReturnModel), StatusCodes.Status200OK)]
        [HttpPost("PostNewScheduleForTask")]
        public async Task<Ok<ScheduleEntityReturnModel>> PostNewScheduleForTask(
            [FromBody] InputScheduleEntityModel inputScheduleEntityModel, 
            [FromServices] IScheduleEntityAppService scheduleEntityAppService)
        {
            var entity = await scheduleEntityAppService.Save(inputScheduleEntityModel);
            var data = ScheduleEntityReturnModel.Create(entity);

            return TypedResults.Ok(data);
        }
    }
}
