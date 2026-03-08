using TimeHacker.Api.Models.Input.Users;
using TimeHacker.Api.Models.Return.Users;
using TimeHacker.Application.Api.Contracts.IAppServices.Users;

namespace TimeHacker.Api.Controllers.Users;

[Authorize]
[ApiController]
[Route("/api/users")]
public class UsersController(IUserAppService userService) : ControllerBase
{
    [ProducesResponseType(typeof(UserReturnModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("me")]
    public async Task<Results<Ok<UserReturnModel>, NotFound>> GetCurrent(CancellationToken cancellationToken = default)
    {
        var user = await userService.GetCurrent(cancellationToken);
        if (user == null)
            return TypedResults.NotFound();

        var data = UserReturnModel.Create(user);
        return TypedResults.Ok(data);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut("me")]
    public async Task<Ok> Update([FromBody] UserUpdateModel inputUserModel, CancellationToken cancellationToken = default)
    {
        await userService.UpdateAsync(inputUserModel.CreateDto(), cancellationToken);

        return TypedResults.Ok();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("me")]
    public async Task<NoContent> Delete(CancellationToken cancellationToken = default)
    {
        await userService.DeleteAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
