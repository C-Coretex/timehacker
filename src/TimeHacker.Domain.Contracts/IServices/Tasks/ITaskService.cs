using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Domain.Contracts.IServices.Tasks
{
    public interface ITaskService
    {
        Task<TasksForDayReturn> GetTasksForDay(DateOnly date);
        IAsyncEnumerable<TasksForDayReturn> GetTasksForDays(IEnumerable<DateOnly> dates);
        IAsyncEnumerable<TasksForDayReturn> RefreshTasksForDays(IEnumerable<DateOnly> dates);
    }
}
