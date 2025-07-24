using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.IRepositories.ScheduleSnapshots
{
    public interface IScheduledTaskRepository: IUserScopedRepositoryBase<ScheduledTask, Guid>
    { }
}
