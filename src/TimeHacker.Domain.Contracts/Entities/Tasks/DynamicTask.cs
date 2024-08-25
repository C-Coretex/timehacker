using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public class DynamicTask : ITask
    {
        public uint Id { get; init; }
        public string UserId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public uint Priority { get; set; }

        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }

        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        public virtual ICollection<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
    }
}
