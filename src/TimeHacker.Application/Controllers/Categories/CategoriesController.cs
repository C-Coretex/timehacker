using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _mapper.ProjectTo<CategoryReturnModel>(_categoryService.GetAll());

            return Ok(data);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = _mapper.Map<CategoryReturnModel>(await _categoryService.GetByIdAsync(id));

            return Ok(data);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] InputCategoryModel inputCategoryModel)
        {
            var category = _mapper.Map<Category>(inputCategoryModel);
            await _categoryService.AddAsync(category);

            return Ok();
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] InputCategoryModel inputCategoryModel)
        {
            var category = new Category()
            {
                Id = id
            };

            //could be done as _ = ..., but this is more readable
            category = _mapper.Map(inputCategoryModel, category);

            await _categoryService.UpdateAsync(category);

            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteAsync(id);

            return Ok();
        }
    }
}
