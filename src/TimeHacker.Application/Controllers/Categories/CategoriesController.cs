using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeHacker.Application.Models.Input.Categories;
using TimeHacker.Application.Models.Return.Categories;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.IServices.Categories;

namespace TimeHacker.Application.Controllers.Categories
{
    [Authorize]
    [ApiController]
    [Route("/api/Categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(IQueryable<CategoryReturnModel>), StatusCodes.Status200OK)]
        [HttpGet("GetAll")]
        public async Task<Ok<IQueryable<CategoryReturnModel>>> GetAll()
        {
            //TODO: to AsAsyncEnumerable
            var data = _mapper.ProjectTo<CategoryReturnModel>(_categoryService.GetAll());

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(CategoryReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetById/{id}")]
        public async Task<Results<Ok<CategoryReturnModel>, NotFound>> GetById(Guid id)
        {
            var entity = await _categoryService.GetByIdAsync(id);
            if (entity == null)
                return TypedResults.NotFound();

            var data = _mapper.Map<CategoryReturnModel>(entity);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Add")]
        public async Task<Ok> Add([FromBody] InputCategoryModel inputCategoryModel)
        {
            var category = _mapper.Map<Category>(inputCategoryModel);
            await _categoryService.AddAsync(category);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update/{id}")]
        public async Task<Ok> Update(Guid id, [FromBody] InputCategoryModel inputCategoryModel)
        {
            var category = new Category()
            {
                Id = id
            };

            //could be done as _ = ..., but this is more readable
            category = _mapper.Map(inputCategoryModel, category);

            await _categoryService.UpdateAsync(category);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete/{id}")]
        public async Task<Ok> Delete(Guid id)
        {
            await _categoryService.DeleteAsync(id);

            return TypedResults.Ok();
        }
    }
}
