namespace TimeHacker.Domain.Contracts.Models.ReturnModels
{
    public class TasksForDayReturn
    {
        public DateOnly Date { get; set; }
        public List<TaskContainerReturn> TasksTimeline { get; set; } = [];
        public List<CategoryContainerReturn> CategoriesTimeline { get; set; } = [];
    }
}
