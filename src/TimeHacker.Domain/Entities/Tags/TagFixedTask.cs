using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.Entities.Tags
{
    public class TagFixedTask
    {
        public Guid TagId { get; init; }
        public Guid TaskId { get; init; }

        public virtual Tag Tag { get; set; } = null!;
        public virtual FixedTask Task { get; set; } = null!;
    }
}
