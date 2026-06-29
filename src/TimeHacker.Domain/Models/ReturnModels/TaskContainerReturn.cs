using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Models.ReturnModels;

public record TaskContainerReturn
{
    public bool IsFixed { get; init; }
    public Guid? ScheduleEntityId { get; init; }
    public required ITask Task { get; init; }
    public TimeRange TimeRange { get; init; }

    public ScheduledTask CreateScheduledTask()
    {
        return new ScheduledTask()
        {
            Start = TimeRange.Start,
            End = TimeRange.End,
            IsFixed = IsFixed,
            UserId = Task.UserId,
            Name = Task.Name,
            Description = Task.Description,
            Priority = Task.Priority,
            ParentTaskId = Task.Id,
            ParentScheduleEntityId = ScheduleEntityId
        };
    }

    /// <summary>
    /// Rebuilds a container from a persisted <see cref="ScheduledTask"/> snapshot row. The snapshot is
    /// denormalized, so the original FixedTask/DynamicTask is NOT reloaded — only a thin shell carrying the
    /// captured display fields (name, description, priority) is reconstructed, keyed by ParentTaskId.
    /// </summary>
    public static TaskContainerReturn Create(ScheduledTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var entity = new TaskContainerReturn
        {
            IsFixed = task.IsFixed,
            ScheduleEntityId = task.ParentTaskId,
            TimeRange = new TimeRange(task.Start, task.End),
            Task = task.IsFixed 
                ? new FixedTask()
                    {
                        Id = task.ParentTaskId,
                    } 
                : new DynamicTask()
                    {
                        Id = task.ParentTaskId,
                    }
        };

        entity.Task.UserId = task.UserId;
        entity.Task.Name = task.Name;
        entity.Task.Description = task.Description;
        entity.Task.Priority = task.Priority;

        return entity;
    }
}