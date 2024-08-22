using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeHacker.Application.Models.Input.Tasks;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;

namespace TimeHacker.Application.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/FixedTasks")]
    public class FixedTasksController: ControllerBase
    {
        private readonly IFixedTaskService _fixedTaskService;
        private readonly ILogger<FixedTasksController> _logger;
        private readonly IMapper _mapper;

        public FixedTasksController(IFixedTaskService fixedTaskService, ILogger<FixedTasksController> logger, IMapper mapper)
        {
            _fixedTaskService = fixedTaskService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _fixedTaskService.GetAll();

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all fixed tasks");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(uint id)
        {
            try
            {
                var data = await _fixedTaskService.GetByIdAsync(id);

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting fixed task by id");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] InputFixedTaskModel inputFixedTaskModel)
        {
            try
            {
                var fixedTask = _mapper.Map<FixedTask>(inputFixedTaskModel);
                await _fixedTaskService.AddAsync(fixedTask);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding fixed task");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(uint id, [FromBody] InputFixedTaskModel inputFixedTaskModel)
        {
            try
            {
                var fixedTask = new FixedTask()
                {
                    Id = id
                };

                //could be done as _ = ..., but this is more readable
                fixedTask = _mapper.Map(inputFixedTaskModel, fixedTask);

                await _fixedTaskService.UpdateAsync(fixedTask);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding fixed task");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(uint id)
        {
            try
            {
                await _fixedTaskService.DeleteAsync(id);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding fixed task");
                return BadRequest(e.Message);
            }
        }
    }
}
