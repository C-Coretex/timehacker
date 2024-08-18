using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots
{
    public interface IScheduleSnapshotService
    {
        Task<ScheduleSnapshot?> GetBy(DateOnly date);
        Task<ScheduleSnapshot> Add(ScheduleSnapshot scheduleSnapshot);
        Task<ScheduleSnapshot> Update(ScheduleSnapshot scheduleSnapshot);
    }
}
