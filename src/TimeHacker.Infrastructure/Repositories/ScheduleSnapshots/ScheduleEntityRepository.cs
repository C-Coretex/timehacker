using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;

public class ScheduleEntityRepository : UserScopedRepositoryBase<ScheduleEntity, Guid>, IScheduleEntityRepository
{
    public ScheduleEntityRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) : base(dbContext, dbContext.ScheduleEntity, userAccessor)
    { }
}
