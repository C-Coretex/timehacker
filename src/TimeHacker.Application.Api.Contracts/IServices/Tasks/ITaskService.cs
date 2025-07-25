using TimeHacker.Domain.Models.ReturnModels;

namespace TimeHacker.Application.Api.Contracts.IServices.Tasks
{
    public interface ITaskService
    {
        Task<TasksForDayReturn> GetTasksForDay(DateOnly date);
        IAsyncEnumerable<TasksForDayReturn> GetTasksForDays(ICollection<DateOnly> dates);
        IAsyncEnumerable<TasksForDayReturn> RefreshTasksForDays(ICollection<DateOnly> dates);
    }
}
