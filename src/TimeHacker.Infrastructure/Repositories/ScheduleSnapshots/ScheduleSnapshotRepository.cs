using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

internal sealed class ScheduleSnapshotRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : UserScopedRepositoryBase<ScheduleSnapshot, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.ScheduleSnapshot, userAccessor, timeProvider), IScheduleSnapshotRepository
{
}
