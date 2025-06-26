using TimeHacker.Api.Models.Return.Tags;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Api.Models.Return.Tasks
{
    public record DynamicTaskReturnModel(
        Guid Id,
        string Name,
        string? Description,
        byte Priority,
        TimeSpan MinTimeToFinish,
        TimeSpan MaxTimeToFinish,
        TimeSpan? OptimalTimeToFinish,
        DateTime CreatedTimestamp,
        IEnumerable<TagReturnModel> Tags
    )
    {
        public static DynamicTaskReturnModel Create(DynamicTask task)
        {
            return new DynamicTaskReturnModel(
                task.Id,
                task.Name,
                task.Description,
                task.Priority,
                task.MinTimeToFinish,
                task.MaxTimeToFinish,
                task.OptimalTimeToFinish,
                task.CreatedTimestamp,
                task.TagDynamicTasks.Select(x => TagReturnModel.Create(x.Tag))
            );
        }
    }
}
