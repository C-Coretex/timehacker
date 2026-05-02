using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.Users;

internal sealed class UserRepository : RepositoryBase<TimeHackerDbContext, User, Guid>, IUserRepository
{
    public UserRepository(TimeHackerDbContext dbContext) 
        : base(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.User)
    { 
    
    }
}
