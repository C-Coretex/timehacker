using TimeHacker.Domain.Contracts.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.IRepositories.Users
{
    public interface IUserRepository : IRepositoryBase<User, string>
    { }
}
