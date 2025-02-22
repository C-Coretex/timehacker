﻿using AutoMapper;
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
                var data = _mapper.ProjectTo<FixedTaskReturnModel>(_fixedTaskService.GetAll());

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all fixed tasks");
                throw;
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var data = _mapper.Map<FixedTaskReturnModel>(await _fixedTaskService.GetByIdAsync(id));

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting fixed task by id");
                throw;
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
                throw;
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] InputFixedTaskModel inputFixedTaskModel)
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
                throw;
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _fixedTaskService.DeleteAsync(id);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding fixed task");
                throw;
            }
        }
    }
}
