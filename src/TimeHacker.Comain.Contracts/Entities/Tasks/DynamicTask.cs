using TimeHacker.Domain.Contracts.Entities.Categories;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public class DynamicTask : ITask
    {
        public int Id { get; init; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public uint Priority { get; set; }

        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }

        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedTimestamp { get; set; } = DateTime.Now;
        public List<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
    }
}
