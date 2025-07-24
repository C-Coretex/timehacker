using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.IRepositories.ScheduleSnapshots
{
    public interface IScheduleSnapshotRepository: IUserScopedRepositoryBase<ScheduleSnapshot, Guid>
    {}
}
