using TimeHacker.Api.Models.Input.Categories;
using TimeHacker.Api.Models.Return.Categories;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;

namespace TimeHacker.Api.Controllers.Categories;

[Authorize]
[ApiController]
[Route("/api/Categories")]
public class CategoriesController(ICategoryAppService categoryService) : ControllerBase
{
    [ProducesResponseType(typeof(IAsyncEnumerable<CategoryReturnModel>), StatusCodes.Status200OK)]
    [HttpGet("GetAll")]
    public Ok<IAsyncEnumerable<CategoryReturnModel>> GetAll(CancellationToken cancellationToken = default)
    {
        var data = categoryService.GetAll(cancellationToken).Select(CategoryReturnModel.Create);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(CategoryReturnModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetById/{id:guid}")]
    public async Task<Results<Ok<CategoryReturnModel>, NotFound>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await categoryService.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return TypedResults.NotFound();

        var data = CategoryReturnModel.Create(entity);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [HttpPost("Add")]
    public async Task<Ok<Guid>> Add([FromBody] InputCategoryModel inputCategoryModel, CancellationToken cancellationToken = default)
    {
        var category = inputCategoryModel.CreateCategory();
        var id = await categoryService.AddAsync(category, cancellationToken);

        return TypedResults.Ok(id);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut("Update/{id:guid}")]
    public async Task<Ok> Update(Guid id, [FromBody] InputCategoryModel inputCategoryModel, CancellationToken cancellationToken = default)
    {
        var category = inputCategoryModel.CreateCategory() with { Id = id };
        await categoryService.UpdateAsync(category, cancellationToken);

        return TypedResults.Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete("Delete/{id:guid}")]
    public async Task<Ok> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await categoryService.DeleteAsync(id, cancellationToken);

        return TypedResults.Ok();
    }
}
