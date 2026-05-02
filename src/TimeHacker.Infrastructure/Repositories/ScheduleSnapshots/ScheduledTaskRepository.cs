using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

internal sealed class ScheduledTaskRepository: UserScopedRepositoryBase<ScheduledTask, Guid>, IScheduledTaskRepository
{
    public ScheduledTaskRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) 
        : base(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.ScheduledTask, userAccessor)
    { }
}
