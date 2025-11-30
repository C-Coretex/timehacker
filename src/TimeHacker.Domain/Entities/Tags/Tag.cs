using System.Drawing;

namespace TimeHacker.Domain.Entities.Tags;

public class Tag : UserScopedEntityBase
{
    public string Name { get; set; } = "";
    public string? Category { get; set; }

    public Color Color { get; set; }

    public virtual ICollection<TagFixedTask> TagFixedTasks { get; set; } = [];
    public virtual ICollection<TagDynamicTask> TagDynamicTasks { get; set; } = [];
}
