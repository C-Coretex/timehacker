using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.Models.EntityModels;
using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.ScheduleSnapshots
{
    public class ScheduleEntity : GuidDbEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public RepeatingEntityModel RepeatingEntity { get; set; }
        public DateOnly? LastEntityCreated { get; set; }
        public DateOnly? EndsOn { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }
        public virtual ICollection<ScheduledCategory> ScheduledCategories { get; set; }

        public virtual FixedTask? FixedTask { get; set; }
        public virtual Category? Category { get; set; }
    }
}
