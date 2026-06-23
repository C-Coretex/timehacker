using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tasks;

internal sealed class DynamicTaskRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : TaskRepository<DynamicTask, Guid>(dbContext, dbContext.DynamicTask, userAccessor, timeProvider), IDynamicTaskRepository
{
}
