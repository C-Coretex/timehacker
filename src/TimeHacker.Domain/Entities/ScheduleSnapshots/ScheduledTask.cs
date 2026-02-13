namespace TimeHacker.Domain.Entities.ScheduleSnapshots;

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
    public bool IsCompleted { get; set; } = false;
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public virtual ScheduleSnapshot ScheduleSnapshot { get; set; } = null!;
    public virtual ScheduledCategory? ScheduledCategory { get; set; }
    public virtual ScheduleEntity? ScheduleEntity { get; set; }
}
