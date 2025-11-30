using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;

namespace TimeHacker.Api.Models.Return.ScheduleSnapshots;

public record ScheduledTaskReturnModel(
    Guid Id,
    Guid ParentTaskId,
    DateOnly Date,
    bool IsFixed,
    string Name,
    string? Description,
    uint Priority,
    bool IsCompleted,
    TimeSpan Start,
    TimeSpan End,
    DateTime? UpdatedTimestamp
)
{
    public static ScheduledTaskReturnModel Create(ScheduledTaskDto scheduledTask)
    {
        return new ScheduledTaskReturnModel(
            scheduledTask.Id!.Value,
            scheduledTask.ParentTaskId,
            scheduledTask.Date,
            scheduledTask.IsFixed,
            scheduledTask.Name,
            scheduledTask.Description,
            scheduledTask.Priority,
            scheduledTask.IsCompleted,
            scheduledTask.Start,
            scheduledTask.End,
            scheduledTask.UpdatedTimestamp
        );
    }
}
