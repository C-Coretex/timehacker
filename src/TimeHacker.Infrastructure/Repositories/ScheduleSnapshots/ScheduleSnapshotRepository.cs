using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduleSnapshotRepository : RepositoryBase<TimeHackerDbContext, ScheduleSnapshot>, IScheduleSnapshotRepository
    {
        public ScheduleSnapshotRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduleSnapshot)
        { }
    }
}
