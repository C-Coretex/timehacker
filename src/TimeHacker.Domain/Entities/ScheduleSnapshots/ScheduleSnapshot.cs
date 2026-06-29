namespace TimeHacker.Domain.Entities.ScheduleSnapshots;

/// <summary>
/// The captured generated state of one user's day (unique per UserId+Date). Task generation
/// involves randomized placement, so persisting the snapshot is what keeps a day's plan stable —
/// re-running generation would otherwise produce a different timeline each time.
/// </summary>
public class ScheduleSnapshot : UserScopedEntityBase
{
    public DateOnly Date { get; set; }

    public virtual ICollection<ScheduledTask> ScheduledTasks { get; init; } = [];
    public virtual ICollection<ScheduledCategory> ScheduledCategories { get; init; } = [];
}
