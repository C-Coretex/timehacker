using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Api.Models.Return.ScheduleSnapshots
{
    public record ScheduledTaskReturnModel(
        ulong Id,
        Guid ParentTaskId,
        DateOnly Date,
        bool IsFixed,
        string Name,
        string? Description,
        uint Priority,
        bool IsCompleted,
        TimeSpan Start,
        TimeSpan End,
        DateTime UpdatedTimestamp
    )
    {
        public static ScheduledTaskReturnModel Create(ScheduledTask scheduledTask)
        {
            return new ScheduledTaskReturnModel(
                scheduledTask.Id,
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
}
