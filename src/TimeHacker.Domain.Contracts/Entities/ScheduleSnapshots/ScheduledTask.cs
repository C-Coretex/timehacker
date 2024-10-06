using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots
{
    public class ScheduledTask : IDbModel<ulong>
    {
        public ulong Id { get; init; }
        public Guid ParentTaskId { get; init; }
        public ulong? ScheduledCategoryId { get; init; }
        public Guid? ParentScheduleEntityId { get; init; }

        public string UserId { get; set; }
        public DateOnly Date { get; set; }

        public bool IsFixed { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public uint Priority { get; set; }
        public bool IsCompleted { get; set; } = false;
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public DateTime UpdatedTimestamp { get; set; }

        public virtual ScheduleSnapshot ScheduleSnapshot { get; set; }
        public virtual ScheduledCategory? ScheduledCategory { get; set; }
        public virtual ScheduleEntity? ScheduleEntity { get; set; }
    }
}
