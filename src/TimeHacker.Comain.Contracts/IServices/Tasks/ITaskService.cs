using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Domain.Contracts.IServices.Tasks
{
    public interface ITaskService
    {
        Task<TasksForDayReturn> GetTasksForDay(DateTime date);
        IAsyncEnumerable<TasksForDayReturn> GetTasksForDays(IEnumerable<DateTime> dates);
        IAsyncEnumerable<TasksForDayReturn> RefreshTasksForDays(IEnumerable<DateTime> dates);
    }
}
