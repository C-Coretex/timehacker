using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeHacker.Api.Models.Input.Users;
using TimeHacker.Api.Models.Return.Users;
using TimeHacker.Application.Api.Contracts.IAppServices.Users;

namespace TimeHacker.Api.Controllers.Users
{
    [Authorize]
    [ApiController]
    [Route("/api/User")]
    public class UsersController(IUserAppService userService) : ControllerBase
    {
        [ProducesResponseType(typeof(UserReturnModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetCurrent")]
        public async Task<Results<Ok<UserReturnModel>, NotFound>> GetCurrent()
        {
            var user = await userService.GetCurrent();
            if (user == null)
                return TypedResults.NotFound();

            var data = UserReturnModel.Create(user);
            return TypedResults.Ok(data);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update")]
        public async Task<Ok> Update([FromBody] UserUpdateModel inputUserModel)
        {
            //TODO: service to DTO, InputModel remains
            await userService.UpdateAsync(inputUserModel.ToDto());

            return TypedResults.Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("Delete")]
        public async Task<Ok> Delete()
        {
            await userService.DeleteAsync();

            return TypedResults.Ok();
        }
    }
}
