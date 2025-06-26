using System.Drawing;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Entities.Tags
{
    public class Tag: IDbModel<Guid>
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string UserId { get; set; }

        public string Name { get; set; }
        public string? Category { get; set; }

        public Color Color { get; set; }

        public virtual ICollection<TagFixedTask> TagFixedTasks { get; set; } = [];
        public virtual ICollection<TagDynamicTask> TagDynamicTasks { get; set; } = [];
    }
}
