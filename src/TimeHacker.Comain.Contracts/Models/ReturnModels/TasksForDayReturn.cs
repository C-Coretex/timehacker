using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Contracts.Models.ReturnModels
{
    public class TaskContainerReturn
    {
        public bool IsFixed { get; set; }
        public ITask Task { get; set; }
        public TimeRange TimeRange { get; set; }
    }
    public class TasksForDayReturn
    {
        public DateOnly Date { get; set; }
        public List<TaskContainerReturn> TasksTimeline { get; set; } = new();
    }
}
