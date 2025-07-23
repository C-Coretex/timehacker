using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.Tasks
{
    public class DynamicTask : GuidDbEntity, ITask
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public byte Priority { get; set; }

        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }

        public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
        public virtual ICollection<TagDynamicTask> TagDynamicTasks { get; set; } = [];
    }
}
