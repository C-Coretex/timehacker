using TimeHacker.Domain.Contracts.Entities.Categories;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public class FixedTask : ITask
    {
        public int Id { get; init; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public uint Priority { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime StartTimestamp { get; set; }

        public DateTime EndTimestamp { get; set; }

        public DateTime CreatedTimestamp { get; set; } = DateTime.Now;

        
        public List<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];
    }
}
