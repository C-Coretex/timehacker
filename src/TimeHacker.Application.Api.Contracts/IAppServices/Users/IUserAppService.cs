using TimeHacker.Application.Api.Contracts.DTOs.Users;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Users
{
    public interface IUserAppService
    {
        Task<UserDto?> GetCurrent();
        Task UpdateAsync(UserDto user);
        Task DeleteAsync();
    }
}
