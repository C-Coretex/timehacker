using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduleSnapshotRepository : RepositoryBase<TimeHackerDbContext, ScheduleSnapshot>, IScheduleSnapshotRepository
    {
        public ScheduleSnapshotRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduleSnapshot)
        { }
    }
}
