using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.Tasks;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;

namespace TimeHacker.Api.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/FixedTasks")]
    public class FixedTasksController(IFixedTaskService fixedTaskService) : ControllerBase
    {
        [ProducesResponseType(typeof(IAsyncEnumerable<FixedTaskReturnModel>), StatusCodes.Status200OK)]
        [HttpGet("GetAll")]
        public Ok<IAsyncEnumerable<FixedTaskReturnModel>> GetAll()
        {
            var data = fixedTaskService.GetAll().AsAsyncEnumerable().Select(FixedTaskReturnModel.Create);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(FixedTaskReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetById/{id:guid}")]
        public async Task<Results<Ok<FixedTaskReturnModel>, NotFound>> GetById(Guid id)
        {
            var entity = await fixedTaskService.GetByIdAsync(id);
            if (entity == null)
                return TypedResults.NotFound();

            var data = FixedTaskReturnModel.Create(entity);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Add")]
        public async Task<Ok> Add([FromBody] InputFixedTaskModel inputFixedTaskModel)
        {
            var fixedTask = inputFixedTaskModel.CreateFixedTask();
            await fixedTaskService.AddAsync(fixedTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update/{id:guid}")]
        public async Task<Ok> Update(Guid id, [FromBody] InputFixedTaskModel inputFixedTaskModel)
        {
            //TODO: when it will be record DTO - will use inputCategoryModel.CreateCategory() with { Id = id };
            var fixedTask = new FixedTask()
            {
                Id = id,
                Name = inputFixedTaskModel.Name,
                Description = inputFixedTaskModel.Description,
                Priority = inputFixedTaskModel.Priority,
                StartTimestamp = DateTime.Parse(inputFixedTaskModel.StartTimestamp),
                EndTimestamp = DateTime.Parse(inputFixedTaskModel.EndTimestamp)
            };
            await fixedTaskService.UpdateAsync(fixedTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete/{id:guid}")]
        public async Task<Ok> Delete(Guid id)
        {
            await fixedTaskService.DeleteAsync(id);

            return TypedResults.Ok();
        }
    }
}
