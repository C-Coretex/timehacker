using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.EntityModels;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots
{
    public class ScheduleEntity : IDbModel<Guid>
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string UserId { get; set; }
        public RepeatingEntityModel RepeatingEntity { get; set; }
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;
        public DateOnly? LastEntityCreated { get; set; }
        public DateOnly? EndsOn { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ICollection<ScheduledCategory> ScheduledCategories { get; set; }

        public virtual FixedTask? FixedTask { get; set; }
        public virtual Category? Category { get; set; }
    }
}
