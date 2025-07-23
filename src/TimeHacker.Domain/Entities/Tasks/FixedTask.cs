using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.Tasks
{
    public class FixedTask : GuidDbEntity, ITask
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

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
