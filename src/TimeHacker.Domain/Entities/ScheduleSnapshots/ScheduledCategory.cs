namespace TimeHacker.Domain.Entities.ScheduleSnapshots;

/// <summary>
/// A generated category-window instance for one specific day, belonging to a <see cref="ScheduleSnapshot"/>.
/// It is a denormalized copy of the originating <see cref="CategorySchedule"/> and its parent
/// <see cref="Category"/>, not either of them: both parent ids are plain columns with no foreign key, so a
/// snapshot reads back without loading anything else and survives the originals being deleted.
/// </summary>
public class ScheduledCategory : UserScopedEntityBase
{
    public Guid ParentCategoryId { get; init; }
    public Guid ParentCategoryScheduleId { get; init; }
    public Guid? ParentScheduleEntity { get; init; }

    public DateOnly Date { get; set; }

    /// <summary>The parent category's name and description — what identifies this band.</summary>
    public required string Name { get; set; }
    public string? Description { get; set; }

    /// <summary>The window's own optional note, copied from <see cref="CategorySchedule.Description"/>.</summary>
    public string? ScheduleDescription { get; set; }

    public Color Color { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public virtual ScheduleSnapshot ScheduleSnapshot { get; set; } = null!;
    public virtual ICollection<ScheduledTask> ScheduledTasks { get; init; } = [];
    public virtual ScheduleEntity? ScheduleEntity { get; set; }
}
