using System.Drawing;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.Categories
{
    public class Category : IDbModel<uint>
    {
        public uint Id { get; init; }
        public string UserId { get; set; }
        public uint? ScheduleEntityId { get; init; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public Color Color { get; set; }


        public virtual ScheduleEntity? ScheduleEntity { get; set; }
        public virtual ICollection<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];
        public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
    }
}
