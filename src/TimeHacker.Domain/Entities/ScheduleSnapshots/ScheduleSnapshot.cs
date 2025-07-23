using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Entities.ScheduleSnapshots
{
    public class ScheduleSnapshot : IDbEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public DateOnly Date { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ICollection<ScheduledCategory> ScheduledCategories { get; set; }
    }
}
