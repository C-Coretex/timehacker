namespace TimeHacker.Domain.Entities.ScheduleSnapshots;

/// <summary>
/// A generated task instance for one specific day, belonging to a <see cref="ScheduleSnapshot"/>. It is a
/// denormalized copy of the originating (<see cref="IsFixed"/>, <see cref="ParentTaskId"/>) task, not the task itself.
/// </summary>
public class ScheduledTask : UserScopedEntityBase
{
    public Guid ParentTaskId { get; init; }
    public Guid? ScheduledCategoryId { get; init; }
    public Guid? ParentScheduleEntityId { get; init; }

    public DateOnly Date { get; set; }

    public bool IsFixed { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public byte Priority { get; set; }
    public bool IsCompleted { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public virtual ScheduleSnapshot ScheduleSnapshot { get; set; } = null!;
    public virtual ScheduledCategory? ScheduledCategory { get; set; }
    public virtual ScheduleEntity? ScheduleEntity { get; set; }
}
