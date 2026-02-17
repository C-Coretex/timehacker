using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Application.Api.QueryPipelineSteps
{
    internal static class QueryPipelineFixedTasks
    {
        public static QueryPipelineStep<FixedTask> IncludeRepeatingData => query =>
        {
            return query.Include(x => x.ScheduleEntity);
        };
    }
}
