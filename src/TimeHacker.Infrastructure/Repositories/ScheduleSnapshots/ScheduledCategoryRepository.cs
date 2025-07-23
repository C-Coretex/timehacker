using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduledCategoryRepository : RepositoryBase<TimeHackerDbContext, ScheduledCategory, Guid>, IScheduledCategoryRepository
    {
        public ScheduledCategoryRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduledCategory)
        { }
    }
}
