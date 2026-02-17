using TimeHacker.Api.Models.Return.RepeatingEntities;
using TimeHacker.Api.Models.Return.ScheduleSnapshots;
using TimeHacker.Api.Models.Return.Tags;
using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Api.Models.Return.Tasks;

public record FixedTaskReturnModel(

    Guid Id,
    string Name,
    string? Description,
    byte Priority,
    DateTime StartTimestamp,
    DateTime EndTimestamp,
    DateTime CreatedTimestamp,
    ScheduleEntityReturnModel? ScheduleEntity,
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
            fixedTask.ScheduleEntity != null ? ScheduleEntityReturnModel.Create(fixedTask.ScheduleEntity) : null,
            fixedTask.Tags.Select(TagReturnModel.Create)
        );
    }
}
