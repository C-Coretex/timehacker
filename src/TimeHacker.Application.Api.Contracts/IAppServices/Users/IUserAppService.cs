using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.Models.InputModels.Users;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Users
{
    public interface IUserAppService
    {
        Task AddAsync(UserUpdateModel user);
        Task<User?> GetCurrent();
        Task UpdateAsync(UserUpdateModel user);
        Task DeleteAsync();
    }
}
