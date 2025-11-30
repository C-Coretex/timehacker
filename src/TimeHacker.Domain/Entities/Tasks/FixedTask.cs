using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tags;

namespace TimeHacker.Domain.Entities.Tasks;

public class FixedTask : UserScopedEntityBase, ITask
{
    public Guid? ScheduleEntityId { get; set; }

    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public byte Priority { get; set; }

    public DateTime StartTimestamp { get; set; }
    public DateTime EndTimestamp { get; set; }

    public virtual ScheduleEntity? ScheduleEntity { get; set; }
    public virtual ICollection<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];
    public virtual ICollection<TagFixedTask> TagFixedTasks { get; set; } = [];

    public FixedTask ShallowCopy() => (FixedTask)MemberwiseClone();
}
