using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeHacker.Application.Models.Input.Tasks;
using TimeHacker.Application.Models.Return.Tasks;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;

namespace TimeHacker.Application.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [Route("/api/DynamicTasks")]
    public class DynamicTasksController : ControllerBase
    {
        private readonly IDynamicTaskService _dynamicTaskService;
        private readonly ILogger<DynamicTasksController> _logger;
        private readonly IMapper _mapper;

        public DynamicTasksController(IDynamicTaskService dynamicTaskService, ILogger<DynamicTasksController> logger, IMapper mapper)
        {
            _dynamicTaskService = dynamicTaskService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _mapper.ProjectTo<DynamicTaskReturnModel>(_dynamicTaskService.GetAll());

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all dynamic tasks");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(uint id)
        {
            try
            {
                var data = _mapper.Map<DynamicTaskReturnModel>( _dynamicTaskService.GetByIdAsync(id));

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting dynamic task by id");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] InputDynamicTaskModel inputDynamicTaskModel)
        {
            try
            {
                var dynamicTask = _mapper.Map<DynamicTask>(inputDynamicTaskModel);
                await _dynamicTaskService.AddAsync(dynamicTask);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding dynamic task");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(uint id, [FromBody] InputDynamicTaskModel inputDynamicTaskModel)
        {
            try
            {
                var dynamicTask = new DynamicTask()
                {
                    Id = id
                };

                //could be done as _ = ..., but this is more readable
                dynamicTask = _mapper.Map(inputDynamicTaskModel, dynamicTask);

                await _dynamicTaskService.UpdateAsync(dynamicTask);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding dynamic task");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(uint id)
        {
            try
            {
                await _dynamicTaskService.DeleteAsync(id);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting dynamic task");
                return BadRequest(e.Message);
            }
        }
    }
}
