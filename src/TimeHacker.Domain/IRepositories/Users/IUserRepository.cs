using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.IRepositories.Users;

public interface IUserRepository : IRepositoryBase<User, Guid>
{ }
