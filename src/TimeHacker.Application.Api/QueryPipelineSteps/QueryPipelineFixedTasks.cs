using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Application.Api.QueryPipelineSteps;

internal static class QueryPipelineFixedTasks
{
    public static QueryPipelineStep<FixedTask> IncludeRepeatingData => query =>
        query.Include(x => x.ScheduleEntity);

    /// <summary>The junction rows alone — enough for a write path to replace the set.</summary>
    public static QueryPipelineStep<FixedTask> IncludeCategoryLinks => query =>
        query.Include(x => x.CategoryFixedTasks);

    /// <summary>
    /// The junctions and the categories behind them, down to each category's windows. A read path needs
    /// the whole chain: unlike the EF projection, the in-memory
    /// <see cref="Contracts.DTOs.Tasks.FixedTaskDto.Selector"/> dereferences the category.
    /// </summary>
    public static QueryPipelineStep<FixedTask> IncludeCategoryData => query =>
        query.Include(x => x.CategoryFixedTasks).ThenInclude(x => x.Category).ThenInclude(x => x.Schedules);
}
