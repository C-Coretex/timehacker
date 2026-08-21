namespace TimeHacker.Domain.Entities.Categories;

public class Category : UserScopedEntityBase
{
    public Guid? ScheduleEntityId { get; set; }

    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public Color Color { get; set; }

    /// <summary>
    /// The daily time window this category occupies. Which day(s) it applies to is decided entirely by
    /// <see cref="ScheduleEntity"/> — a category with no schedule is never placed on the calendar.
    /// </summary>
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public virtual ScheduleEntity? ScheduleEntity { get; set; }
    public virtual ICollection<CategoryFixedTask> CategoryFixedTasks { get; init; } = [];
    public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; init; } = [];
}
