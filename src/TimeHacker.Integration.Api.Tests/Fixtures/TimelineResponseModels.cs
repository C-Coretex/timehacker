using System.Drawing;
using TimeHacker.Domain.Models.BusinessLogicModels;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

// Client-side mirror of TasksForDayDto. The server DTO's TaskContainerDto.Task is the ITask interface,
// which STJ cannot deserialize on the client, so tests read into these concrete shapes instead.
public sealed record TimelineDayResponse
{
    public DateOnly Date { get; init; }
    public IReadOnlyList<TimelineTaskResponse> TasksTimeline { get; init; } = [];
    public IReadOnlyList<TimelineCategoryResponse> CategoriesTimeline { get; init; } = [];
}

public sealed record TimelineTaskResponse
{
    public bool IsFixed { get; init; }
    public Guid? ScheduleEntityId { get; init; }
    public required TimelineTaskInfo Task { get; init; }
    public TimeRange TimeRange { get; init; }
}

public sealed record TimelineTaskInfo
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public byte Priority { get; init; }
}

public sealed record TimelineCategoryResponse
{
    public Guid? ScheduleEntityId { get; init; }
    public required TimelineCategoryInfo Category { get; init; }
    public TimeRange TimeRange { get; init; }
}

public sealed record TimelineCategoryInfo
{
    public Guid? Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public Color Color { get; init; }
}
