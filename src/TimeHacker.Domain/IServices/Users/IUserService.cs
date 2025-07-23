using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.Models.InputModels.Users;

namespace TimeHacker.Domain.IServices.Users
{
    public interface IUserService
    {
        Task AddAsync(UserUpdateModel user);
        Task<User?> GetCurrent();
        Task UpdateAsync(UserUpdateModel user);
        Task DeleteAsync();
    }
}
