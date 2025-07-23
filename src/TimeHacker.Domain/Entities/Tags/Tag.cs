using System.Drawing;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.Tags
{
    public class Tag : GuidDbEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public string Name { get; set; }
        public string? Category { get; set; }

        public Color Color { get; set; }

        public virtual ICollection<TagFixedTask> TagFixedTasks { get; set; } = [];
        public virtual ICollection<TagDynamicTask> TagDynamicTasks { get; set; } = [];
    }
}
