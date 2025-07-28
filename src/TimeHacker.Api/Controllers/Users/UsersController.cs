using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeHacker.Api.Models.Return.Users;
using TimeHacker.Application.Api.Contracts.IAppServices.Users;
using TimeHacker.Domain.Models.InputModels.Users;

namespace TimeHacker.Api.Controllers.Users
{
    [Authorize]
    [ApiController]
    [Route("/api/User")]
    public class UsersController : ControllerBase
    {
        private readonly IUserAppService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IUserAppService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [ProducesResponseType(typeof(UserReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetCurrent")]
        public async Task<Results<Ok<UserReturnModel>, NotFound>> GetCurrent()
        {
            var user = await _userService.GetCurrent();
            if (user == null)
                return TypedResults.NotFound();

            var data = UserReturnModel.Create(user);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Add")]
        public async Task<Ok> Add([FromBody] UserUpdateModel inputUserModel)
        {
            var context = _httpContextAccessor.HttpContext;
            var userIdentityId = context!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            //TODO: service to DTO, InputModel remains
            await _userService.AddAsync(inputUserModel, userIdentityId);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update")]
        public async Task<Ok> Update([FromBody] UserUpdateModel inputUserModel)
        {
            //TODO: service to DTO, InputModel remains
            await _userService.UpdateAsync(inputUserModel);

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete")]
        public async Task<Ok> Delete()
        {
            await _userService.DeleteAsync();

            return TypedResults.Ok();
        }
    }
}
