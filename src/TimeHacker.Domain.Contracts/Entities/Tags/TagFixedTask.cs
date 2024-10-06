using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.Entities.Tags
{
    public class TagFixedTask
    {
        public Guid TagId { get; init; }
        public Guid TaskId { get; init; }

        public virtual Tag Tag { get; set; } = null!;
        public virtual FixedTask Task { get; set; } = null!;
    }
}
