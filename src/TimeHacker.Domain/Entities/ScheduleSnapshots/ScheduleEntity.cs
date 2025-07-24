﻿using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.Models.EntityModels;

namespace TimeHacker.Domain.Entities.ScheduleSnapshots
{
    public class ScheduleEntity : UserScopedEntityBase
    {
        public RepeatingEntityModel RepeatingEntity { get; set; }
        public DateOnly? LastEntityCreated { get; set; }
        public DateOnly? EndsOn { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ICollection<ScheduledCategory> ScheduledCategories { get; set; }

        public virtual FixedTask? FixedTask { get; set; }
        public virtual Category? Category { get; set; }
    }
}
