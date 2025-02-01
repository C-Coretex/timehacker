using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduledCategoryRepository : RepositoryBase<TimeHackerDbContext, ScheduledCategory, ulong>, IScheduledCategoryRepository
    {
        public ScheduledCategoryRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduledCategory)
        { }
    }
}
