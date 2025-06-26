using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Domain.Services.IncludeExpansionDelegates
{
    public static class IncludeExpansionScheduleSnapshots
    {
        public static IncludeExpansionDelegate<ScheduleSnapshot> IncludeScheduledTasks => query =>
        {
            return query.Include(x => x.ScheduledTasks);
        };
        

        public static IncludeExpansionDelegate<ScheduleSnapshot> IncludeScheduledCategories => query =>
        {
            return query.Include(x => x.ScheduledCategories);
        };
    }
}
