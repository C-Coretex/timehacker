using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Contracts.Models.ReturnModels
{
    public record TaskBase: ITask
    {
        public int Id { get; init; }
        public string UserId { get ; set ; }
        public string Name { get ; set ; }
        public string? Description { get ; set ; }
        public uint Priority { get ; set ; }
        public bool IsCompleted { get ; set ; }
        public DateTime CreatedTimestamp { get ; set ; }

        public TaskBase() { }

        public TaskBase(ITask task)
        {
            Id = task.Id;
            UserId = task.UserId;
            Name = task.Name;
            Description = task.Description;
            Priority = task.Priority;
            IsCompleted = task.IsCompleted;
            CreatedTimestamp = task.CreatedTimestamp;
        }
    }

    public class TaskContainerReturn
    {
        public bool IsFixed { get; set; }
        public TaskBase Task { get; set; }
        public TimeRange TimeRange { get; set; }
    }


    public class CategoryContainerReturn
    {
        public Category? Category { get; set; }
        public TimeRange TimeRange { get; set; }
    }


    public class TasksForDayReturn
    {
        public DateOnly Date { get; set; }
        public List<TaskContainerReturn> TasksTimeline { get; set; } = new();
        public List<CategoryContainerReturn> CategoriesTimeline { get; set; } = new();
    }
}
