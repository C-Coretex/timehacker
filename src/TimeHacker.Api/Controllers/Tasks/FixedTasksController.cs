using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

namespace TimeHacker.Api.Controllers.Tasks;

[Authorize]
[ApiController]
[Route("/api/fixed-tasks")]
public class FixedTasksController(IFixedTaskAppService fixedTaskAppService) : ControllerBase
{
    [ProducesResponseType(typeof(IAsyncEnumerable<FixedTaskReturnModel>), StatusCodes.Status200OK)]
    [HttpGet]
    public Ok<IAsyncEnumerable<FixedTaskReturnModel>> GetAll(CancellationToken cancellationToken = default)
    {
        var data = fixedTaskAppService.GetAll(cancellationToken).Select(FixedTaskReturnModel.Create);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(FixedTaskReturnModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<Results<Ok<FixedTaskReturnModel>, NotFound>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await fixedTaskAppService.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return TypedResults.NotFound();

        var data = FixedTaskReturnModel.Create(entity);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<Created<Guid>> Add([FromBody] InputFixedTaskModel inputFixedTaskModel, CancellationToken cancellationToken = default)
    {
        var fixedTaskDto = inputFixedTaskModel.CreateDto();
        var id = await fixedTaskAppService.AddAsync(fixedTaskDto, cancellationToken);

        return TypedResults.Created($"/api/fixed-tasks/{id}", id);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}")]
    public async Task<Ok> Update(Guid id, [FromBody] InputFixedTaskModel inputFixedTaskModel, CancellationToken cancellationToken = default)
    {
        var fixedTaskDto = inputFixedTaskModel.CreateDto() with { Id = id };
        await fixedTaskAppService.UpdateAsync(fixedTaskDto, cancellationToken);

        return TypedResults.Ok();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:guid}")]
    public async Task<NoContent> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await fixedTaskAppService.DeleteAsync(id, cancellationToken);

        return TypedResults.NoContent();
    }
}
