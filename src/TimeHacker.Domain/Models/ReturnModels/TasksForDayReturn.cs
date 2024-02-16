using TimeHacker.Domain.Models.BusinessLogicModels;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Domain.Models.ReturnModels
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
