using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.Tasks;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IServices.Tasks;

namespace TimeHacker.Api.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/DynamicTasks")]
    public class DynamicTasksController(IDynamicTaskService dynamicTaskService) : ControllerBase
    {
        [ProducesResponseType(typeof(IAsyncEnumerable<DynamicTaskReturnModel>), StatusCodes.Status200OK)]
        [HttpGet("GetAll")]
        public Ok<IAsyncEnumerable<DynamicTaskReturnModel>> GetAll()
        {
            var data = dynamicTaskService.GetAll().AsAsyncEnumerable().Select(DynamicTaskReturnModel.Create);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(DynamicTaskReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetById/{id:guid}")]
        public async Task<Results<Ok<DynamicTaskReturnModel>, NotFound>> GetById(Guid id)
        {
            var entity = await dynamicTaskService.GetByIdAsync(id);
            if (entity == null)
                return TypedResults.NotFound();

            var data = DynamicTaskReturnModel.Create(entity);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Add")]
        public async Task<Ok> Add([FromBody] InputDynamicTaskModel inputDynamicTaskModel)
        {
            var dynamicTask = inputDynamicTaskModel.CreateDynamicTask();
            await dynamicTaskService.AddAsync(dynamicTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update/{id:guid}")]
        public async Task<Ok> Update(Guid id, [FromBody] InputDynamicTaskModel inputDynamicTaskModel)
        {
            //TODO: when it will be record DTO - will use inputCategoryModel.CreateCategory() with { Id = id };

            var dynamicTask = new DynamicTask()
            {
                Id = id,
                Name = inputDynamicTaskModel.Name,
                Description = inputDynamicTaskModel.Description,
                Priority = inputDynamicTaskModel.Priority,
                MinTimeToFinish = inputDynamicTaskModel.MinTimeToFinish,
                MaxTimeToFinish = inputDynamicTaskModel.MaxTimeToFinish,
                OptimalTimeToFinish = inputDynamicTaskModel.OptimalTimeToFinish
            };

            await dynamicTaskService.UpdateAsync(dynamicTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete/{id:guid}")]
        public async Task<Ok> Delete(Guid id)
        {
            await dynamicTaskService.DeleteAsync(id);

            return TypedResults.Ok();
        }
    }
}
