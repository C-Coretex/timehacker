using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.Entities.Categories
{
    public class CategoryDynamicTask
    {
        public uint CategoryId { get; set; }
        public uint DynamicTaskId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual DynamicTask DynamicTask { get; set; } = null!;
    }
}
