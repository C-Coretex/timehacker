using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

internal sealed class ScheduledTaskRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : UserScopedRepositoryBase<ScheduledTask, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.ScheduledTask, userAccessor, timeProvider), IScheduledTaskRepository
{
}
