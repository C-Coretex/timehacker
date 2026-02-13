using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.Entities.Categories;

public class CategoryDynamicTask
{
    public Guid CategoryId { get; init; }
    public Guid DynamicTaskId { get; init; }

    public virtual Category Category { get; set; } = null!;
    public virtual DynamicTask DynamicTask { get; set; } = null!;
}
