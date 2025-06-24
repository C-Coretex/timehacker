using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [ProducesResponseType(typeof(IQueryable<DynamicTaskReturnModel>), StatusCodes.Status200OK)]
        [HttpGet("GetAll")]
        public async Task<Ok<IQueryable<DynamicTaskReturnModel>>> GetAll()
        {
            //TODO: to AsEnumerableAsync
            var data = _mapper.ProjectTo<DynamicTaskReturnModel>(_dynamicTaskService.GetAll());

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(DynamicTaskReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetById/{id}")]
        public async Task<Results<Ok<DynamicTaskReturnModel>, NotFound>> GetById(Guid id)
        {
            var entity = await _dynamicTaskService.GetByIdAsync(id);
            if (entity == null)
                return TypedResults.NotFound();

            var data = _mapper.Map<DynamicTaskReturnModel>(entity);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Add")]
        public async Task<Ok> Add([FromBody] InputDynamicTaskModel inputDynamicTaskModel)
        {
            var dynamicTask = _mapper.Map<DynamicTask>(inputDynamicTaskModel);
            await _dynamicTaskService.AddAsync(dynamicTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update/{id}")]
        public async Task<Ok> Update(Guid id, [FromBody] InputDynamicTaskModel inputDynamicTaskModel)
        {
            var dynamicTask = new DynamicTask()
            {
                Id = id
            };

            //could be done as _ = ..., but this is more readable
            dynamicTask = _mapper.Map(inputDynamicTaskModel, dynamicTask);

            await _dynamicTaskService.UpdateAsync(dynamicTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete/{id}")]
        public async Task<Ok> Delete(Guid id)
        {
            await _dynamicTaskService.DeleteAsync(id);

            return TypedResults.Ok();
        }
    }
}
