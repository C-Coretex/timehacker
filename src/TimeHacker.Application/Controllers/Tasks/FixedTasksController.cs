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
    [Route("/api/FixedTasks")]
    public class FixedTasksController: ControllerBase
    {
        private readonly IFixedTaskService _fixedTaskService;
        private readonly IMapper _mapper;

        public FixedTasksController(IFixedTaskService fixedTaskService, IMapper mapper)
        {
            _fixedTaskService = fixedTaskService;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(IQueryable<FixedTaskReturnModel>), StatusCodes.Status200OK)]
        [HttpGet("GetAll")]
        public async Task<Ok<IQueryable<FixedTaskReturnModel>>> GetAll()
        {
            //TODO: to AsEnumerableAsync
            var data = _mapper.ProjectTo<FixedTaskReturnModel>(_fixedTaskService.GetAll());

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(typeof(FixedTaskReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetById/{id}")]
        public async Task<Results<Ok<FixedTaskReturnModel>, NotFound>> GetById(Guid id)
        {
            var entity = await _fixedTaskService.GetByIdAsync(id);
            if (entity == null)
                return TypedResults.NotFound();

            var data = _mapper.Map<FixedTaskReturnModel>(entity);

            return TypedResults.Ok(data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Add")]
        public async Task<Ok> Add([FromBody] InputFixedTaskModel inputFixedTaskModel)
        {
            var fixedTask = _mapper.Map<FixedTask>(inputFixedTaskModel);
            await _fixedTaskService.AddAsync(fixedTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update/{id}")]
        public async Task<Ok> Update(Guid id, [FromBody] InputFixedTaskModel inputFixedTaskModel)
        {
            var fixedTask = new FixedTask()
            {
                Id = id
            };

            //could be done as _ = ..., but this is more readable
            fixedTask = _mapper.Map(inputFixedTaskModel, fixedTask);

            await _fixedTaskService.UpdateAsync(fixedTask);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete/{id}")]
        public async Task<Ok> Delete(Guid id)
        {
            await _fixedTaskService.DeleteAsync(id);

            return TypedResults.Ok();
        }
    }
}
