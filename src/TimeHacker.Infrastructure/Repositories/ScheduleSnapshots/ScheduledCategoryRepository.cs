namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

internal sealed class ScheduledCategoryRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : UserScopedRepositoryBase<ScheduledCategory, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.ScheduledCategory, userAccessor, timeProvider), IScheduledCategoryRepository
{
}
