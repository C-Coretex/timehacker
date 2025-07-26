using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Api.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/DynamicTasks")]
    public class DynamicTasksController(IDynamicTaskAppService dynamicTaskAppService) : ControllerBase
    {
        [ProducesResponseType(typeof(IAsyncEnumerable<DynamicTaskReturnModel>), StatusCodes.Status200OK)]
        [HttpGet("GetAll")]
        public Ok<IAsyncEnumerable<DynamicTaskReturnModel>> GetAll()
        {
            var data = dynamicTaskAppService.GetAll().Select(DynamicTaskReturnModel.Create);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(DynamicTaskReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetById/{id:guid}")]
        public async Task<Results<Ok<DynamicTaskReturnModel>, NotFound>> GetById(Guid id)
        {
            var entity = await dynamicTaskAppService.GetByIdAsync(id);
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
            await dynamicTaskAppService.AddAsync(dynamicTask);

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

            await dynamicTaskAppService.UpdateAsync(dynamicTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete/{id:guid}")]
        public async Task<Ok> Delete(Guid id)
        {
            await dynamicTaskAppService.DeleteAsync(id);

            return TypedResults.Ok();
        }
    }
}
