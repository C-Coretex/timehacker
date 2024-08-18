using System;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots
{
    public class ScheduledTask : IDbModel<Guid>
    {
        public Guid Id { get; init; }
        public int ParentTaskId { get; init; }
        public Guid? ScheduledCategoryId { get; init; }

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
    }
}
