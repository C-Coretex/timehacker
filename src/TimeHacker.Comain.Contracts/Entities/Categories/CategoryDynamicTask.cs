using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.Entities.Categories
{
    public class CategoryDynamicTask
    {
        public int CategoryId { get; set; }
        public int DynamicTaskId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual DynamicTask DynamicTask { get; set; } = null!;
    }
}
