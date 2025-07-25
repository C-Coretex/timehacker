using TimeHacker.Application.Api.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Services.ScheduleSnapshots
{
    public class ScheduledTaskService(IScheduledTaskRepository scheduledTaskRepository)
        : IScheduledTaskService
    {
        public Task<ScheduledTask?> GetBy(Guid id)
        {
            return scheduledTaskRepository.GetByIdAsync(id);
        }
    }
}
