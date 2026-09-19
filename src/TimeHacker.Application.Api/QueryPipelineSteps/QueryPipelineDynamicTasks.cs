using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Application.Api.QueryPipelineSteps;

internal static class QueryPipelineDynamicTasks
{
    /// <summary>The junction rows alone — enough for a write path to replace the set.</summary>
    public static QueryPipelineStep<DynamicTask> IncludeCategoryLinks => query =>
        query.Include(x => x.CategoryDynamicTasks);

    /// <summary>
    /// The junctions and the categories behind them, down to each category's windows. A read path needs
    /// the whole chain: unlike the EF projection, the in-memory
    /// <see cref="Contracts.DTOs.Tasks.DynamicTaskDto.Selector"/> dereferences the category.
    /// </summary>
    public static QueryPipelineStep<DynamicTask> IncludeCategoryData => query =>
        query.Include(x => x.CategoryDynamicTasks).ThenInclude(x => x.Category).ThenInclude(x => x.Schedules);
}
