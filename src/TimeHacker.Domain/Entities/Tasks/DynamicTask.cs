using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.Entities.Tags;

namespace TimeHacker.Domain.Entities.Tasks
{
    public class DynamicTask : UserScopedEntityBase, ITask
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public byte Priority { get; set; }

        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }

        public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
        public virtual ICollection<TagDynamicTask> TagDynamicTasks { get; set; } = [];
    }
}
