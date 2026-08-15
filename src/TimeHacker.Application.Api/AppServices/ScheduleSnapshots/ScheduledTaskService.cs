using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;

namespace TimeHacker.Application.Api.AppServices.ScheduleSnapshots;

public class ScheduledTaskAppService(IScheduledTaskRepository scheduledTaskRepository)
    : IScheduledTaskAppService
{
    public async Task<ScheduledTaskDto?> GetBy(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await scheduledTaskRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        return entity != null ? ScheduledTaskDto.Create(entity) : null;
    }
}
