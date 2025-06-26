using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.IServices.ScheduleSnapshots
{
    public interface IScheduledTaskService
    {
        Task<ScheduledTask?> GetBy(ulong id);
    }
}
