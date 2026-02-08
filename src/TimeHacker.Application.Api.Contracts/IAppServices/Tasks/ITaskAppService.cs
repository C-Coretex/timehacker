using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

public interface ITaskAppService
{
    Task<TasksForDayDto> GetTasksForDay(DateOnly date, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TasksForDayDto> GetTasksForDays(ICollection<DateOnly> dates, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TasksForDayDto> RefreshTasksForDays(ICollection<DateOnly> dates, CancellationToken cancellationToken = default);
}
