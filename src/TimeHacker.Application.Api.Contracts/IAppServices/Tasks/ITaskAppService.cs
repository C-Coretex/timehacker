using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Tasks
{
    public interface ITaskAppService
    {
        Task<TasksForDayDto> GetTasksForDay(DateOnly date);
        IAsyncEnumerable<TasksForDayDto> GetTasksForDays(ICollection<DateOnly> dates);
        IAsyncEnumerable<TasksForDayDto> RefreshTasksForDays(ICollection<DateOnly> dates);
    }
}
