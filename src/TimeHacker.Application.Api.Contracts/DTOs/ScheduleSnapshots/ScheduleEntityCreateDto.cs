using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;

public record ScheduleEntityCreateDto(
    ScheduleEntityParentEnum ScheduleEntityParentEnum,
    Guid ParentEntityId,
    RepeatingEntityDto RepeatingEntityModel,
    EndsOnModel? EndsOnModel = null
    )
{
}
