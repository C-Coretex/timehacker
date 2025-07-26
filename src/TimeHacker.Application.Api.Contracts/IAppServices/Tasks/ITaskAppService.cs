using TimeHacker.Domain.Models.ReturnModels;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Tasks
{
    public interface ITaskAppService
    {
        Task<TasksForDayReturn> GetTasksForDay(DateOnly date);
        IAsyncEnumerable<TasksForDayReturn> GetTasksForDays(ICollection<DateOnly> dates);
        IAsyncEnumerable<TasksForDayReturn> RefreshTasksForDays(ICollection<DateOnly> dates);
    }
}
