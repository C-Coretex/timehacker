using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots
{
    public interface IScheduleSnapshotService
    {
        Task<ScheduleSnapshot?> GetBy(DateOnly date);
        Task Add(ScheduleSnapshot scheduleSnapshot);
        Task Update(ScheduleSnapshot scheduleSnapshot);

        Task<ScheduledTask?> GetScheduledTaskBy(ulong id);
    }
}
