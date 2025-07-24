using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.Services.ScheduleSnapshots
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
