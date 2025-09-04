using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.IRepositories.ScheduleSnapshots
{
    public interface IScheduleEntityRepository: IUserScopedRepositoryBase<ScheduleEntity, Guid>
    { }
}
