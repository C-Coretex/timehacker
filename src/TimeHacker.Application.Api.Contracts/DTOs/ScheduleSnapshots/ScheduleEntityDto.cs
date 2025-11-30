using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;

public record ScheduleEntityDto(
    Guid? Id,
    RepeatingEntityDto RepeatingEntity,
    DateTime CreatedTimestamp,
    DateOnly? LastEntityCreated,
    DateOnly? EndsOn
    )
{
    public static ScheduleEntityDto Create(ScheduleEntity scheduleEntity)
    {
        return new ScheduleEntityDto(
            scheduleEntity.Id,
            scheduleEntity.RepeatingEntity,
            scheduleEntity.CreatedTimestamp,
            scheduleEntity.LastEntityCreated,
            scheduleEntity.EndsOn
        );
    }
}
