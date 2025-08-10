using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Application.Api.AppServices.ScheduleSnapshots
{
    public class ScheduledTaskAppService(IScheduledTaskRepository scheduledTaskRepository)
        : IScheduledTaskAppService
    {
        public async Task<ScheduledTaskDto?> GetBy(Guid id)
        {
            var entity = await scheduledTaskRepository.GetByIdAsync(id);
            return entity != null ? ScheduledTaskDto.Create(entity) : null;
        }
    }
}
