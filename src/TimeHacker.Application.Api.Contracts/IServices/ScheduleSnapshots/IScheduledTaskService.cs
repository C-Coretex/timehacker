using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.IServices.ScheduleSnapshots
{
    public interface IScheduledTaskService
    {
        Task<ScheduledTask?> GetBy(Guid id);
    }
}
