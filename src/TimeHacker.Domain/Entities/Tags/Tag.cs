using System.Drawing;
using TimeHacker.Domain.Entities.EntityBase;

namespace TimeHacker.Domain.Entities.Tags
{
    public class Tag : UserScopedEntityBase
    {
        public required string Name { get; set; }
        public string? Category { get; set; }

        public Color Color { get; set; }

        public virtual ICollection<TagFixedTask> TagFixedTasks { get; set; } = [];
        public virtual ICollection<TagDynamicTask> TagDynamicTasks { get; set; } = [];
    }
}
