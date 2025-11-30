using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.Entities.Tags;

public class TagDynamicTask
{
    public Guid TagId { get; init; }
    public Guid TaskId { get; init; }

    public virtual Tag Tag { get; set; } = null!;
    public virtual DynamicTask Task { get; set; } = null!;
}
