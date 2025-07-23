using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduleEntityRepository : RepositoryBase<TimeHackerDbContext, ScheduleEntity, Guid>, IScheduleEntityRepository
    {
        public ScheduleEntityRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduleEntity)
        { }
    }
}
