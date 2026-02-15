using TimeHacker.Api.Models.Return.RepeatingEntities;
using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;

namespace TimeHacker.Api.Models.Return.ScheduleSnapshots;

public record ScheduleEntityReturnModel(
    Guid Id,
    ReturnRepeatingEntityModelBase RepeatingEntity,
    DateTime ScheduleCreated,
    DateOnly? LastTaskCreated,
    DateOnly? EndsOn
)
{
    public static ScheduleEntityReturnModel Create(ScheduleEntityDto scheduleEntity)
    {
        return new ScheduleEntityReturnModel(
            scheduleEntity.Id!.Value,
            ReturnRepeatingEntityModelBase.Create(scheduleEntity.RepeatingEntity),
            scheduleEntity.CreatedTimestamp,
            scheduleEntity.LastEntityCreated,
            scheduleEntity.EndsOn
        );
    }
}
