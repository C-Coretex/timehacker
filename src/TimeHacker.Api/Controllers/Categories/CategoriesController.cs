using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Api.Models.Input.Categories;
using TimeHacker.Api.Models.Return.Categories;
using TimeHacker.Application.Api.Contracts.IServices.Categories;
using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Api.Controllers.Categories
{
    [Authorize]
    [ApiController]
    [Route("/api/Categories")]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        [ProducesResponseType(typeof(IAsyncEnumerable<CategoryReturnModel>), StatusCodes.Status200OK)]
        [HttpGet("GetAll")]
        public Ok<IAsyncEnumerable<CategoryReturnModel>> GetAll()
        {
            var data = categoryService.GetAll().AsAsyncEnumerable().Select(CategoryReturnModel.Create);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(CategoryReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetById/{id:guid}")]
        public async Task<Results<Ok<CategoryReturnModel>, NotFound>> GetById(Guid id)
        {
            var entity = await categoryService.GetByIdAsync(id);
            if (entity == null)
                return TypedResults.NotFound();

            var data = CategoryReturnModel.Create(entity);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Add")]
        public async Task<Ok> Add([FromBody] InputCategoryModel inputCategoryModel)
        {
            var category = inputCategoryModel.CreateCategory();
            await categoryService.AddAsync(category);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update/{id:guid}")]
        public async Task<Ok> Update(Guid id, [FromBody] InputCategoryModel inputCategoryModel)
        {
            //TODO: when it will be record DTO - will use inputCategoryModel.CreateCategory() with { Id = id };
            var category = new Category()
            {
                Id = id,
                Name = inputCategoryModel.Name,
                Description = inputCategoryModel.Description,
                Color = inputCategoryModel.Color
            };
            await categoryService.UpdateAsync(category);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete/{id:guid}")]
        public async Task<Ok> Delete(Guid id)
        {
            await categoryService.DeleteAsync(id);

            return TypedResults.Ok();
        }
    }
}
