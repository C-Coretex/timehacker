using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.ScheduleSnapshots
{
    public class ScheduledTask : GuidDbEntity
    {
        public Guid ParentTaskId { get; init; }
        public Guid? ScheduledCategoryId { get; init; }
        public Guid? ParentScheduleEntityId { get; init; }

        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public DateOnly Date { get; set; }

        public bool IsFixed { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public byte Priority { get; set; }
        public bool IsCompleted { get; set; } = false;
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public DateTime UpdatedTimestamp { get; set; }

        public virtual ScheduleSnapshot ScheduleSnapshot { get; set; }
        public virtual ScheduledCategory? ScheduledCategory { get; set; }
        public virtual ScheduleEntity? ScheduleEntity { get; set; }
    }
}
