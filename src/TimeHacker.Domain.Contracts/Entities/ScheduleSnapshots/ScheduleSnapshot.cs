﻿using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots
{
    public class ScheduleSnapshot : IDbModel
    {
        public string UserId { get; set; }
        public DateOnly Date { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ICollection<ScheduledCategory> ScheduledCategories { get; set; }
    }
}
