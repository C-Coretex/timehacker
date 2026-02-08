using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;

public interface IScheduleEntityAppService
{
    Task<ScheduleEntityDto> Save(ScheduleEntityCreateDto scheduleEntityCreateDto, CancellationToken cancellationToken = default);
}
