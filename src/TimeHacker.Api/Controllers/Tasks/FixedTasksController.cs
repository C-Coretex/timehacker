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
    public Ok<IAsyncEnumerable<FixedTaskReturnModel>> GetAll(CancellationToken cancellationToken = default)
    {
        var data = fixedTaskAppService.GetAll(cancellationToken).Select(FixedTaskReturnModel.Create);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(FixedTaskReturnModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetById/{id:guid}")]
    public async Task<Results<Ok<FixedTaskReturnModel>, NotFound>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await fixedTaskAppService.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return TypedResults.NotFound();

        var data = FixedTaskReturnModel.Create(entity);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [HttpPost("Add")]
    public async Task<Ok<Guid>> Add([FromBody] InputFixedTaskModel inputFixedTaskModel, CancellationToken cancellationToken = default)
    {
        var fixedTaskDto = inputFixedTaskModel.CreateFixedTaskDto();
        var id = await fixedTaskAppService.AddAsync(fixedTaskDto, cancellationToken);

        return TypedResults.Ok(id);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut("Update/{id:guid}")]
    public async Task<Ok> Update(Guid id, [FromBody] InputFixedTaskModel inputFixedTaskModel, CancellationToken cancellationToken = default)
    {
        //TODO: when it will be record DTO - will use inputCategoryModel.CreateCategory() with { Id = id };
        var fixedTaskDto = inputFixedTaskModel.CreateFixedTaskDto() with { Id = id };
        await fixedTaskAppService.UpdateAsync(fixedTaskDto, cancellationToken);

        return TypedResults.Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete("Delete/{id:guid}")]
    public async Task<Ok> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await fixedTaskAppService.DeleteAsync(id, cancellationToken);

        return TypedResults.Ok();
    }
}
