﻿using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Domain.IncludeExpansionDelegates
{
    public static class IncludeExpansionScheduleEntity
    {
        public static IncludeExpansionDelegate<ScheduleEntity> IncludeFixedTask => query =>
        {
            return query.Include(x => x.FixedTask);
        };
    }
}
