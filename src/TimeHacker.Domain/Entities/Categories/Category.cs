namespace TimeHacker.Domain.Entities.Categories;

public class Category : UserScopedEntityBase
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public Color Color { get; set; }

    public virtual ICollection<CategorySchedule> Schedules { get; init; } = [];
    public virtual ICollection<CategoryFixedTask> CategoryFixedTasks { get; init; } = [];
    public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; init; } = [];
}
