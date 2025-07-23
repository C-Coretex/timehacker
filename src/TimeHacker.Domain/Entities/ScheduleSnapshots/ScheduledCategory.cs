using System.Drawing;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.ScheduleSnapshots
{
    public class ScheduledCategory : GuidDbEntity
    {
        public Guid ParentCategoryId { get; init; }
        public Guid? ParentScheduleEntity { get; init; }

        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

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
