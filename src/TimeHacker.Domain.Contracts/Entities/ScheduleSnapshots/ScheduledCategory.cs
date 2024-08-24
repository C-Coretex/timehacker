using System.Drawing;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots
{
    public class ScheduledCategory : IDbModel<ulong>
    {
        public ulong Id { get; init; }
        public uint ParentCategoryId { get; init; }
        public uint? ParentScheduleEntity { get; init; }

        public string UserId { get; set; }
        public DateOnly Date { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public DateTime UpdatedTimestamp { get; set; }

        public virtual ScheduleSnapshot ScheduleSnapshot { get; set; }
        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ScheduleEntity? ScheduleEntity { get; set; }
    }
}
