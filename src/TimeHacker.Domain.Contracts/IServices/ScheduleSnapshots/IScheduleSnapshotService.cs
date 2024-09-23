using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots
{
    public interface IScheduleSnapshotService
    {
        Task<ScheduleSnapshot?> GetByAsync(DateOnly date);
        Task<ScheduleSnapshot> AddAsync(ScheduleSnapshot scheduleSnapshot);
        Task<ScheduleSnapshot> UpdateAsync(ScheduleSnapshot scheduleSnapshot);
    }
}
