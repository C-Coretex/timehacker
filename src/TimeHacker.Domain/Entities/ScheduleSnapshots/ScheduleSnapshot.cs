namespace TimeHacker.Domain.Entities.ScheduleSnapshots
{
    public class ScheduleSnapshot : UserScopedEntityBase
    {
        public DateOnly Date { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ICollection<ScheduledCategory> ScheduledCategories { get; set; }
    }
}
