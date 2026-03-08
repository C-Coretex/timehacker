using TimeHacker.Api.Models.Input.Categories;
using TimeHacker.Api.Models.Return.Categories;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;

namespace TimeHacker.Api.Controllers.Categories;

[Authorize]
[ApiController]
[Route("/api/categories")]
public class CategoriesController(ICategoryAppService categoryService) : ControllerBase
{
    [ProducesResponseType(typeof(IAsyncEnumerable<CategoryReturnModel>), StatusCodes.Status200OK)]
    [HttpGet]
    public Ok<IAsyncEnumerable<CategoryReturnModel>> GetAll(CancellationToken cancellationToken = default)
    {
        var data = categoryService.GetAll(cancellationToken).Select(CategoryReturnModel.Create);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(CategoryReturnModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<Results<Ok<CategoryReturnModel>, NotFound>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await categoryService.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return TypedResults.NotFound();

        var data = CategoryReturnModel.Create(entity);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<Created<Guid>> Add([FromBody] InputCategoryModel inputCategoryModel, CancellationToken cancellationToken = default)
    {
        var category = inputCategoryModel.CreateDto();
        var id = await categoryService.AddAsync(category, cancellationToken);

        return TypedResults.Created($"/api/categories/{id}", id);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}")]
    public async Task<Ok> Update(Guid id, [FromBody] InputCategoryModel inputCategoryModel, CancellationToken cancellationToken = default)
    {
        var category = inputCategoryModel.CreateDto() with { Id = id };
        await categoryService.UpdateAsync(category, cancellationToken);

        return TypedResults.Ok();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:guid}")]
    public async Task<NoContent> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await categoryService.DeleteAsync(id, cancellationToken);

        return TypedResults.NoContent();
    }
}
