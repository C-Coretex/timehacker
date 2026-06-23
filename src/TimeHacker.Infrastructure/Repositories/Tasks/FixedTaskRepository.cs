using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tasks;

internal sealed class FixedTaskRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : TaskRepository<FixedTask, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.FixedTask, userAccessor, timeProvider), IFixedTaskRepository
{
}
