using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduledTaskRepository: UserScopedRepositoryBase<ScheduledTask, Guid>, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) : base(dbContext, dbContext.ScheduledTask, userAccessor)
        { }
    }
}
