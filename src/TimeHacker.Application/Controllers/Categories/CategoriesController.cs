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
    public class CategoriesController: ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger, IMapper mapper)
        {
            _categoryService = categoryService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _mapper.ProjectTo<CategoryReturnModel>(_categoryService.GetAll());

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all categories");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(uint id)
        {
            try
            {
                var data = _mapper.Map<CategoryReturnModel>(await _categoryService.GetByIdAsync(id));

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting category by id");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] InputCategoryModel inputCategoryModel)
        {
            try
            {
                var category = _mapper.Map<Category>(inputCategoryModel);
                await _categoryService.AddAsync(category);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding category");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(uint id, [FromBody] InputCategoryModel inputCategoryModel)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding category");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(uint id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding category");
                return BadRequest(e.Message);
            }
        }
    }
}
