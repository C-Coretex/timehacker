using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

internal sealed class ScheduledCategoryRepository : UserScopedRepositoryBase<ScheduledCategory, Guid>, IScheduledCategoryRepository
{
    public ScheduledCategoryRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) 
        : base(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.ScheduledCategory, userAccessor)
    { }
}
