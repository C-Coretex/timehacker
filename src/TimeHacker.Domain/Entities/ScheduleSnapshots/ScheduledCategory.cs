using System.Drawing;

namespace TimeHacker.Domain.Entities.ScheduleSnapshots
{
    public class ScheduledCategory : UserScopedEntityBase
    {
        public Guid ParentCategoryId { get; init; }
        public Guid? ParentScheduleEntity { get; init; }


        public DateOnly Date { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        public Color Color { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public virtual ScheduleSnapshot ScheduleSnapshot { get; set; }
        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ScheduleEntity? ScheduleEntity { get; set; }
    }
}
