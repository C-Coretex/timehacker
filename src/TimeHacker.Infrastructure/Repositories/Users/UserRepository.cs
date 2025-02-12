using TimeHacker.Domain.Contracts.Entities.Users;
using TimeHacker.Domain.Contracts.IRepositories.Users;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.Users
{
    public class UserRepository : RepositoryBase<TimeHackerDbContext, User, string>, IUserRepository
    {
        public UserRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.User)
        { }
    }
}
