using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots
{
    public interface IScheduledTaskAppService
    {
        Task<ScheduledTask?> GetBy(Guid id);
    }
}
