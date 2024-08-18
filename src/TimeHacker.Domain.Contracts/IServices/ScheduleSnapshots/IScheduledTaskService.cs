using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots
{
    public interface IScheduledTaskService
    {
        Task<ScheduledTask?> GetBy(Guid id);
    }
}
