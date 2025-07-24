using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduledCategoryRepository : UserScopedRepositoryBase<ScheduledCategory, Guid>, IScheduledCategoryRepository
    {
        public ScheduledCategoryRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) : base(dbContext, dbContext.ScheduledCategory, userAccessor)
        { }
    }
}
