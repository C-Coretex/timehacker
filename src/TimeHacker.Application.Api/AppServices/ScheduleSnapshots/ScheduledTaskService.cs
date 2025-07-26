using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Application.Api.AppServices.ScheduleSnapshots
{
    public class ScheduledTaskAppService(IScheduledTaskRepository scheduledTaskRepository)
        : IScheduledTaskAppService
    {
        public Task<ScheduledTask?> GetBy(Guid id)
        {
            return scheduledTaskRepository.GetByIdAsync(id);
        }
    }
}
