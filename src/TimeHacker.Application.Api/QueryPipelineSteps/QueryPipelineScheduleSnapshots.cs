using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Application.Api.QueryPipelineSteps;

internal static class QueryPipelineScheduleSnapshots
{
    public static QueryPipelineStep<ScheduleSnapshot> IncludeScheduledData => query =>
    {
        return query.Include(x => x.ScheduledTasks)
                    .Include(x => x.ScheduledCategories);
    };
}
