using TimeHacker.Domain.IRepositories.Users;

namespace TimeHacker.Infrastructure.Repositories.Users;

internal sealed class UserRepository(TimeHackerDbContext dbContext, TimeProvider timeProvider) 
    : RepositoryBase<TimeHackerDbContext, User, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.User, timeProvider), IUserRepository
{
}
