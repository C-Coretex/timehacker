namespace TimeHacker.Domain.Entities.Categories;

/// <summary>
/// One dated, wall-clock time window belonging to a <see cref="Categories.Category"/>. The category is the
/// organisational label tasks link to and supplies the name and colour; a window only says when in the day
/// it applies. A category may own several windows, including more than one on the same date, and they are
/// free to overlap.
/// </summary>
public class CategorySchedule : UserScopedEntityBase
{
    public Guid CategoryId { get; set; }
    public Guid? ScheduleEntityId { get; set; }

    /// <summary>
    /// Optional note distinguishing this window from the category's others, e.g. "Working hours" vs
    /// "Overtime" under a "Work" category. The category's own name is what identifies it.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>The day this window applies to; an attached schedule, if any, repeats it after that day.</summary>
    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public virtual Category Category { get; set; } = null!;
    public virtual ScheduleEntity? ScheduleEntity { get; set; }
}
