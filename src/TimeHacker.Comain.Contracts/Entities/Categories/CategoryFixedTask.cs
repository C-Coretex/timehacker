using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.Entities.Categories
{
    public class CategoryFixedTask
    {
        public int CategoryId { get; set; }
        public int FixedTaskId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual FixedTask FixedTask { get; set; } = null!;
    }
}
