using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeHacker.Application.Models.Input.Categories;
using TimeHacker.Application.Models.Return.Users;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.IServices.Users;
using TimeHacker.Domain.Contracts.Models.InputModels.Users;
using TimeHacker.Domain.Services.Users;

namespace TimeHacker.Application.Controllers.Users
{
    [Authorize]
    [ApiController]
    [Route("/api/User")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, ILogger<UsersController> logger, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetCurrent")]
        public async Task<IActionResult> GetCurrent()
        {
            try
            {
                var data = _mapper.Map<UserReturnModel>(await _userService.GetCurrent());

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting current user");
                throw;
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] UserUpdateModel inputUserModel)
        {
            try
            {
                await _userService.AddAsync(inputUserModel);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding user");
                throw;
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateModel inputUserModel)
        {
            try
            {
                await _userService.UpdateAsync(inputUserModel);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating user");
                throw;
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete()
        {
            try
            {
                await _userService.DeleteAsync();

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting user");
                throw;
            }
        }
    }
}
