using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

internal sealed class ScheduleEntityRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : UserScopedRepositoryBase<ScheduleEntity, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.ScheduleEntity, userAccessor, timeProvider), IScheduleEntityRepository
{
}
