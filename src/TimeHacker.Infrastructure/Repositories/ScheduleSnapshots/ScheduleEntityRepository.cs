using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.DB.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduleEntityRepository : RepositoryBase<TimeHackerDbContext, ScheduleEntity, Guid>, IScheduleEntityRepository
    {
        public ScheduleEntityRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduleEntity)
        { }
    }
}
