using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

namespace TimeHacker.Api.Controllers.Tasks;

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

    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [HttpPost("Add")]
    public async Task<Ok<Guid>> Add([FromBody] InputDynamicTaskModel inputDynamicTaskModel)
    {
        var dynamicTask = inputDynamicTaskModel.CreateDynamicTaskDto();
        var id = await dynamicTaskAppService.AddAsync(dynamicTask);

        return TypedResults.Ok(id);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut("Update/{id:guid}")]
    public async Task<Ok> Update(Guid id, [FromBody] InputDynamicTaskModel inputDynamicTaskModel)
    {
        var dynamicTaskDto = inputDynamicTaskModel.CreateDynamicTaskDto() with { Id = id };
        await dynamicTaskAppService.UpdateAsync(dynamicTaskDto);

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
