using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

internal sealed class ScheduleEntityRepository : UserScopedRepositoryBase<ScheduleEntity, Guid>, IScheduleEntityRepository
{
    public ScheduleEntityRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) 
        : base(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.ScheduleEntity, userAccessor)
    { }
}
