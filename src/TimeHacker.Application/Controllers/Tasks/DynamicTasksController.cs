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
        private readonly IMapper _mapper;

        public DynamicTasksController(IDynamicTaskService dynamicTaskService, IMapper mapper)
        {
            _dynamicTaskService = dynamicTaskService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            //TODO: to AsEnumerableAsync
            var data = _mapper.ProjectTo<DynamicTaskReturnModel>(_dynamicTaskService.GetAll());

            return Ok(data);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = _mapper.Map<DynamicTaskReturnModel>(await _dynamicTaskService.GetByIdAsync(id));

            return Ok(data);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] InputDynamicTaskModel inputDynamicTaskModel)
        {
            var dynamicTask = _mapper.Map<DynamicTask>(inputDynamicTaskModel);
            await _dynamicTaskService.AddAsync(dynamicTask);

            return Ok();
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] InputDynamicTaskModel inputDynamicTaskModel)
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

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _dynamicTaskService.DeleteAsync(id);

            return Ok();
        }
    }
}
