using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

internal sealed class ScheduleSnapshotRepository : UserScopedRepositoryBase<ScheduleSnapshot, Guid>, IScheduleSnapshotRepository
{
    public ScheduleSnapshotRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) 
        : base(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.ScheduleSnapshot, userAccessor)
    { }
}
