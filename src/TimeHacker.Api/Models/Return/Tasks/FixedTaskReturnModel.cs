using TimeHacker.Api.Models.Return.Tags;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Api.Models.Return.Tasks
{
    public record FixedTaskReturnModel(
        
        Guid Id,
        string Name,
        string? Description,
        byte Priority,
        DateTime StartTimestamp,
        DateTime EndTimestamp,
        DateTime CreatedTimestamp,
        IEnumerable<TagReturnModel> Tags
    )
    {
        public static FixedTaskReturnModel Create(FixedTask fixedTask)
        {
            return new FixedTaskReturnModel(
                fixedTask.Id,
                fixedTask.Name,
                fixedTask.Description,
                fixedTask.Priority,
                fixedTask.StartTimestamp,
                fixedTask.EndTimestamp,
                fixedTask.CreatedTimestamp,
                fixedTask.TagFixedTasks.Select(x => TagReturnModel.Create(x.Tag))
            );
        }
    }
}
