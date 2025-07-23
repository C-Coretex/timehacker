using System.Drawing;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.Categories
{
    public class Category : GuidDbEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public Guid? ScheduleEntityId { get; init; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public Color Color { get; set; }


        public virtual ScheduleEntity? ScheduleEntity { get; set; }
        public virtual ICollection<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];
        public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
    }
}
