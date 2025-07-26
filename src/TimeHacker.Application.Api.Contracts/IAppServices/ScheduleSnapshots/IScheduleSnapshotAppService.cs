using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots
{
    public interface IScheduleSnapshotAppService
    {
        Task<ScheduleSnapshot?> GetByAsync(DateOnly date);
        Task<ScheduleSnapshot> AddAsync(ScheduleSnapshot scheduleSnapshot);
        Task<ScheduleSnapshot> UpdateAsync(ScheduleSnapshot scheduleSnapshot);
    }
}
