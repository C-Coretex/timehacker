namespace TimeHacker.Domain.Entities.ScheduleSnapshots;

/// <summary>
/// The recurrence *blueprint* attached to a FixedTask or Category: the repeating pattern plus the bounds
/// (first/last generated date, optional end). It is not a task instance — instances are the
/// <see cref="ScheduledTask"/>/<see cref="ScheduledCategory"/> rows generated from it for specific days.
/// </summary>
public class ScheduleEntity : UserScopedEntityBase
{
    public RepeatingEntityDto RepeatingEntity { get; set; } = null!;
    public DateOnly? FirstEntityCreated { get; set; }
    public DateOnly? LastEntityCreated { get; set; }
    public DateOnly? EndsOn { get; set; }

    public virtual ICollection<ScheduledTask> ScheduledTasks { get; init; } = [];
    public virtual ICollection<ScheduledCategory> ScheduledCategories { get; init; } = [];

    public virtual FixedTask? FixedTask { get; set; }
    public virtual Category? Category { get; set; }
}
