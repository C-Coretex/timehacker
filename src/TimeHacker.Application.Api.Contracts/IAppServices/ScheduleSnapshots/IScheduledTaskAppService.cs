using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots
{
    public interface IScheduledTaskAppService
    {
        Task<ScheduledTaskDto?> GetBy(Guid id);
    }
}
