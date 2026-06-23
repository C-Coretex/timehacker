using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.Users;

internal sealed class UserRepository(TimeHackerDbContext dbContext, TimeProvider timeProvider) 
    : RepositoryBase<TimeHackerDbContext, User, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.User, timeProvider), IUserRepository
{
}
