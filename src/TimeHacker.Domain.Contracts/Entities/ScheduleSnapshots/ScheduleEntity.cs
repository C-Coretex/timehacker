﻿using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.EntityModels;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots
{
    public class ScheduleEntity : IDbModel<uint>
    {
        public uint Id { get; init; }

        public string UserId { get; set; }
        public RepeatingEntityModel RepeatingEntity { get; set; }
        public DateTime ScheduleCreated { get; set; } = DateTime.UtcNow;
        public DateTime LastTaskCreated { get; set; }
        public DateOnly? EndsOn { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ICollection<ScheduledCategory> ScheduledCategories { get; set; }

        public virtual FixedTask? FixedTask { get; set; }
        public virtual Category? Category { get; set; }
    }
}
