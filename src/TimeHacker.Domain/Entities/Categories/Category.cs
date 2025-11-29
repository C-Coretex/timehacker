using System.Drawing;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Entities.Categories
{
    public class Category : UserScopedEntityBase
    {
        public Guid? ScheduleEntityId { get; set; }

        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public Color Color { get; set; }


        public virtual ScheduleEntity? ScheduleEntity { get; set; }
        public virtual ICollection<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];
        public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
    }
}
