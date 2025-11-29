using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.IProcessors
{
    public interface ITaskTimelineProcessor
    {
        TasksForDayReturn GetTasksForDay(IEnumerable<FixedTask> fixedTasks, IEnumerable<FixedTask> scheduledFixedTasks, IEnumerable<DynamicTask> dynamicTasks, DateOnly date);

    }
}
