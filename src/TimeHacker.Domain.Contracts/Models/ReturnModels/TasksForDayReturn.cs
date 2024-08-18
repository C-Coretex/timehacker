using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Contracts.Models.ReturnModels
{
    public class TaskContainerReturn
    {
        public Guid Id { get; set; }
        public bool IsFixed { get; set; }
        public ITask Task { get; set; }
        public TimeRange TimeRange { get; set; }
    }

    public class CategoryContainerReturn
    {
        public Guid Id { get; set; }
        public Category? Category { get; set; }
        public TimeRange TimeRange { get; set; }
    }


    public class TasksForDayReturn
    {
        public DateOnly Date { get; set; }
        public List<TaskContainerReturn> TasksTimeline { get; set; } = [];
        public List<CategoryContainerReturn> CategoriesTimeline { get; set; } = [];
    }
}
