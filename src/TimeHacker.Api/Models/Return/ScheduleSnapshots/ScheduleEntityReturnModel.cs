using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Domain.DTOs.RepeatingEntity;

namespace TimeHacker.Api.Models.Return.ScheduleSnapshots;

public record ScheduleEntityReturnModel(
    Guid Id,
    RepeatingEntityDto RepeatingEntity,
    DateTime ScheduleCreated,
    DateOnly? LastTaskCreated,
    DateOnly? EndsOn
)
{
    public static ScheduleEntityReturnModel Create(ScheduleEntityDto scheduleEntity)
    {
        return new ScheduleEntityReturnModel(
            scheduleEntity.Id!.Value,
            scheduleEntity.RepeatingEntity,
            scheduleEntity.CreatedTimestamp,
            scheduleEntity.LastEntityCreated,
            scheduleEntity.EndsOn
        );
    }
}
