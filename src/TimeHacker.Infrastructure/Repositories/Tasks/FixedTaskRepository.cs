using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tasks;

internal sealed class FixedTaskRepository : TaskRepository<FixedTask, Guid>, IFixedTaskRepository
{
    public FixedTaskRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) 
        : base(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.FixedTask, userAccessor)
    { }
}
