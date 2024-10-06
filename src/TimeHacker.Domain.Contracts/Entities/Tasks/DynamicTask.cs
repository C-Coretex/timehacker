using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.Tags;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public class DynamicTask : ITask
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string UserId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public byte Priority { get; set; }

        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }

        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
        public virtual ICollection<TagDynamicTask> TagDynamicTasks { get; set; } = [];
    }
}
