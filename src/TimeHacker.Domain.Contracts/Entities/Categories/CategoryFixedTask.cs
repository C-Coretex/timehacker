using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.Entities.Categories
{
    public class CategoryFixedTask
    {
        public Guid CategoryId { get; init; }
        public Guid FixedTaskId { get; init; }

        public virtual Category Category { get; set; } = null!;
        public virtual FixedTask FixedTask { get; set; } = null!;
    }
}
