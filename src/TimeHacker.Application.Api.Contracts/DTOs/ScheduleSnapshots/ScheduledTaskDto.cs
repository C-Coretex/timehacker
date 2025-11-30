using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;

public record ScheduledTaskDto(
    Guid? Id,
    Guid ParentTaskId,
    Guid? ScheduledCategoryId,
    Guid? ParentScheduleEntityId,
    DateOnly Date,
    bool IsFixed,
    string Name,
    string? Description,
    byte Priority,
    bool IsCompleted,
    TimeSpan Start,
    TimeSpan End,
    DateTime? UpdatedTimestamp)
{
    public static ScheduledTaskDto Create(ScheduledTask scheduledTask)
    {
        return new ScheduledTaskDto(
            scheduledTask.Id,
            scheduledTask.ParentTaskId,
            scheduledTask.ScheduledCategoryId,
            scheduledTask.ParentScheduleEntityId,
            scheduledTask.Date,
            scheduledTask.IsFixed,
            scheduledTask.Name,
            scheduledTask.Description,
            scheduledTask.Priority,
            scheduledTask.IsCompleted,
            scheduledTask.Start,
            scheduledTask.End,
            scheduledTask.UpdatedTimestamp);
    }
}
