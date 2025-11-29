using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduleSnapshotRepository : UserScopedRepositoryBase<ScheduleSnapshot, Guid>, IScheduleSnapshotRepository
    {
        public ScheduleSnapshotRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) : base(dbContext, dbContext.ScheduleSnapshot, userAccessor)
        { }
    }
}
