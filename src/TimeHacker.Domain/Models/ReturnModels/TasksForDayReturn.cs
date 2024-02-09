using TimeHacker.Domain.Models.Tasks;

namespace TimeHacker.Domain.Models.ReturnModels
{
    public class TasksForDayReturn
    {
        public IEnumerable<DynamicTask> DynamicTasks { get; set; }
        public IEnumerable<FixedTask> FixedTasks { get; set; }
    }
}
