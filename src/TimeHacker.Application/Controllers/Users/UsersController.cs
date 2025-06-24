using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeHacker.Application.Models.Return.Users;
using TimeHacker.Domain.Contracts.IServices.Users;
using TimeHacker.Domain.Contracts.Models.InputModels.Users;

namespace TimeHacker.Application.Controllers.Users
{
    [Authorize]
    [ApiController]
    [Route("/api/User")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("GetCurrent")]
        public async Task<Results<Ok<UserReturnModel>, NotFound>> GetCurrent()
        {
            var user = await _userService.GetCurrent();
            if (user == null)
                return TypedResults.NotFound();

            var data = _mapper.Map<UserReturnModel>(user);
            return TypedResults.Ok(data);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] UserUpdateModel inputUserModel)
        {
            await _userService.AddAsync(inputUserModel);

            return Ok();
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateModel inputUserModel)
        {
            await _userService.UpdateAsync(inputUserModel);

            return Ok();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete()
        {
            await _userService.DeleteAsync();

            return Ok();
        }
    }
}
