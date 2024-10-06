using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tags;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public class FixedTask : ITask
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid? ScheduleEntityId { get; init; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public byte Priority { get; set; }

        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        public virtual ScheduleEntity? ScheduleEntity { get; set; }
        public virtual ICollection<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];
        public virtual ICollection<TagFixedTask> TagFixedTasks { get; set; } = [];

        public FixedTask ShallowCopy() => (FixedTask)MemberwiseClone();
    }
}
