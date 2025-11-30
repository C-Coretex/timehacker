using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.Users;

public class UserRepository : RepositoryBase<TimeHackerDbContext, User, Guid>, IUserRepository
{
    public UserRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.User)
    { }
}
