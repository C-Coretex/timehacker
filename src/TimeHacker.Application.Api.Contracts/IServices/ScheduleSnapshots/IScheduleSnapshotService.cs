using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.IServices.ScheduleSnapshots
{
    public interface IScheduleSnapshotService
    {
        Task<ScheduleSnapshot?> GetByAsync(DateOnly date);
        Task<ScheduleSnapshot> AddAsync(ScheduleSnapshot scheduleSnapshot);
        Task<ScheduleSnapshot> UpdateAsync(ScheduleSnapshot scheduleSnapshot);
    }
}
