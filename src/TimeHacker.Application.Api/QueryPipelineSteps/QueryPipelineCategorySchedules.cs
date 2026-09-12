using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Application.Api.QueryPipelineSteps;

internal static class QueryPipelineCategorySchedules
{
    // The timeline reads the parent's colour and description for every band, so a window is never
    // useful without its category.
    public static QueryPipelineStep<CategorySchedule> IncludeCategory => query =>
        query.Include(x => x.Category);
}
