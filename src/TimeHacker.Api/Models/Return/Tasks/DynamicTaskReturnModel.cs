using TimeHacker.Api.Models.Return.Categories;
using TimeHacker.Api.Models.Return.Tags;

namespace TimeHacker.Api.Models.Return.Tasks;

public record DynamicTaskReturnModel(
    Guid? Id,
    string Name,
    string? Description,
    byte Priority,
    TimeSpan MinTimeToFinish,
    TimeSpan MaxTimeToFinish,
    TimeSpan? OptimalTimeToFinish,
    DateTime CreatedTimestamp,
    IEnumerable<CategoryReturnModel> Categories,
    IEnumerable<TagReturnModel> Tags
)
{
    public static DynamicTaskReturnModel Create(DynamicTaskDto task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new DynamicTaskReturnModel(
            task.Id,
            task.Name,
            task.Description,
            task.Priority,
            task.MinTimeToFinish,
            task.MaxTimeToFinish,
            task.OptimalTimeToFinish,
            task.CreatedTimestamp,
            task.RichCategories.Select(CategoryReturnModel.Create),
            task.Tags.Select(TagReturnModel.Create)
        );
    }
}
