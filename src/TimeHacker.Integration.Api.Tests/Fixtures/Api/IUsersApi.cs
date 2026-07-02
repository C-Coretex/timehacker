using Refit;
using TimeHacker.Api.Models.Input.Users;
using TimeHacker.Api.Models.Return.Users;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

/// <summary>Typed surface over <c>UsersController</c> (the current-user /api/users/me resource).</summary>
public interface IUsersApi
{
    [Get("/api/users/me")]
    Task<IApiResponse<UserReturnModel>> GetCurrent();

    [Put("/api/users/me")]
    Task<IApiResponse> UpdateCurrent([Body] UserUpdateModel model);

    [Delete("/api/users/me")]
    Task<IApiResponse> DeleteCurrent();
}
