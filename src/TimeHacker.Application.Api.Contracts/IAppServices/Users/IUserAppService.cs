using TimeHacker.Application.Api.Contracts.DTOs.Users;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Users;

public interface IUserAppService
{
    Task<UserDto?> GetCurrent(CancellationToken cancellationToken = default);
    Task UpdateAsync(UserDto user, CancellationToken cancellationToken = default);
    Task DeleteAsync(CancellationToken cancellationToken = default);
}
