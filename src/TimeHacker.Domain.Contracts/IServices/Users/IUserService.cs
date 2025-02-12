using TimeHacker.Domain.Contracts.Entities.Users;
using TimeHacker.Domain.Contracts.Models.InputModels.Users;

namespace TimeHacker.Domain.Contracts.IServices.Users
{
    public interface IUserService
    {
        Task AddAsync(UserUpdateModel user);
        Task<User?> GetCurrent();
        Task UpdateAsync(UserUpdateModel user);
        Task DeleteAsync();
    }
}
