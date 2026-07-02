using Refit;
using TimeHacker.Api.Models.Input.Tasks;
using TimeHacker.Api.Models.Return.ScheduleSnapshots;
using TimeHacker.Api.Models.Return.Tasks;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

/// <summary>Typed surface over <c>TasksController</c> (/api/tasks): timeline, schedules and scheduled tasks.</summary>
public interface ITasksApi
{
    // Timeline responses are read into test-local models (TimelineDayResponse) because the server's
    // TaskContainerDto exposes an ITask interface that STJ can't deserialize on the client.
    [Get("/api/tasks/timeline/day")]
    Task<IApiResponse<TimelineDayResponse>> GetForDay(string date);

    [Get("/api/tasks/timeline")]
    Task<IApiResponse<IReadOnlyList<TimelineDayResponse>>> GetForDays([Query(CollectionFormat.Multi)] IEnumerable<string> dates);

    [Post("/api/tasks/timeline/refresh")]
    Task<IApiResponse<IReadOnlyList<TimelineDayResponse>>> RefreshForDays([Body] ICollection<DateOnly> dates);

    [Get("/api/tasks/scheduled/{id}")]
    Task<IApiResponse<ScheduledTaskReturnModel>> GetScheduled(Guid id);

    [Post("/api/tasks/schedules")]
    Task<IApiResponse<ScheduleEntityReturnModel>> CreateSchedule([Body] InputScheduleEntityModel model);
}
