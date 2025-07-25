﻿using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Models.EntityModels;

namespace TimeHacker.Api.Models.Return.ScheduleSnapshots
{
    public record ScheduleEntityReturnModel(
        Guid Id,
        RepeatingEntityModel RepeatingEntity,
        DateTime ScheduleCreated,
        DateOnly? LastTaskCreated,
        DateOnly? EndsOn
    )
    {
        public static ScheduleEntityReturnModel Create(ScheduleEntity scheduleEntity)
        {
            return new ScheduleEntityReturnModel(
                scheduleEntity.Id,
                scheduleEntity.RepeatingEntity,
                scheduleEntity.CreatedTimestamp,
                scheduleEntity.LastEntityCreated,
                scheduleEntity.EndsOn
            );
        }
    }
}
