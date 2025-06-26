using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduledTaskRepository: RepositoryBase<TimeHackerDbContext, ScheduledTask, ulong>, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduledTask)
        { }
    }
}
