using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

namespace TimeHacker.Api.Controllers.Tasks;

[Authorize]
[ApiController]
[Route("/api/FixedTasks")]
public class FixedTasksController(IFixedTaskAppService fixedTaskAppService) : ControllerBase
{
    [ProducesResponseType(typeof(IAsyncEnumerable<FixedTaskReturnModel>), StatusCodes.Status200OK)]
    [HttpGet("GetAll")]
    public Ok<IAsyncEnumerable<FixedTaskReturnModel>> GetAll()
    {
        var data = fixedTaskAppService.GetAll().Select(FixedTaskReturnModel.Create);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(FixedTaskReturnModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetById/{id:guid}")]
    public async Task<Results<Ok<FixedTaskReturnModel>, NotFound>> GetById(Guid id)
    {
        var entity = await fixedTaskAppService.GetByIdAsync(id);
        if (entity == null)
            return TypedResults.NotFound();

        var data = FixedTaskReturnModel.Create(entity);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("Add")]
    public async Task<Ok> Add([FromBody] InputFixedTaskModel inputFixedTaskModel)
    {
        var fixedTaskDto = inputFixedTaskModel.CreateFixedTaskDto();
        await fixedTaskAppService.AddAsync(fixedTaskDto);

        return TypedResults.Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut("Update/{id:guid}")]
    public async Task<Ok> Update(Guid id, [FromBody] InputFixedTaskModel inputFixedTaskModel)
    {
        //TODO: when it will be record DTO - will use inputCategoryModel.CreateCategory() with { Id = id };
        var fixedTaskDto = inputFixedTaskModel.CreateFixedTaskDto() with { Id = id };
        await fixedTaskAppService.UpdateAsync(fixedTaskDto);

        return TypedResults.Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete("Delete/{id:guid}")]
    public async Task<Ok> Delete(Guid id)
    {
        await fixedTaskAppService.DeleteAsync(id);

        return TypedResults.Ok();
    }
}
