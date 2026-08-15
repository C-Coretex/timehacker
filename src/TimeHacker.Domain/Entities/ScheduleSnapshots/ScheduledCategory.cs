namespace TimeHacker.Domain.Entities.ScheduleSnapshots;

/// <summary>
/// A generated category instance for one specific day, belonging to a <see cref="ScheduleSnapshot"/>. It is a
/// denormalized copy of the originating <see cref="ParentCategoryId"/> category, not the category itself.
/// </summary>
public class ScheduledCategory : UserScopedEntityBase
{
    public Guid ParentCategoryId { get; init; }
    public Guid? ParentScheduleEntity { get; init; }

    public DateOnly Date { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public Color Color { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public virtual ScheduleSnapshot ScheduleSnapshot { get; set; } = null!;
    public virtual ICollection<ScheduledTask> ScheduledTasks { get; init; } = [];
    public virtual ScheduleEntity? ScheduleEntity { get; set; }
}
