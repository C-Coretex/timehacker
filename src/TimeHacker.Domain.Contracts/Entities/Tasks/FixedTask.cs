using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public class FixedTask : ITask
    {
        public uint Id { get; init; }
        public string UserId { get; set; }
        public uint? ScheduleEntityId { get; init; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public uint Priority { get; set; }

        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        public virtual ScheduleEntity? ScheduleEntity { get; set; }
        public virtual ICollection<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];
    }
}
