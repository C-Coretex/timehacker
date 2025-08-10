using TimeHacker.Api.Models.Return.Tags;
using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Api.Models.Return.Tasks
{
    public class FixedTaskReturnModel(
        Guid Id,
        string Name,
        string? Description,
        byte Priority,
        DateTime StartTimestamp,
        DateTime EndTimestamp,
        DateTime CreatedTimestamp,
        IEnumerable<TagReturnModel> Tags)
    {
        public static FixedTaskReturnModel Create(FixedTaskDto fixedTask)
        {
            return new FixedTaskReturnModel(
                fixedTask.Id!.Value,
                fixedTask.Name,
                fixedTask.Description,
                fixedTask.Priority,
                fixedTask.StartTimestamp,
                fixedTask.EndTimestamp,
                fixedTask.CreatedTimestamp,
                fixedTask.Tags.Select(TagReturnModel.Create)
            );
        }
    }
}
