using TimeHacker.Domain.IProcessors;
using TimeHacker.Domain.Services.Processors;

namespace TimeHacker.Domain.Services.Tests.ProcessorTests;

public class TaskTimelineProcessorTests
{
    private readonly ITaskTimelineProcessor _processor = new TaskTimelineProcessor();

    [Fact]
    [Trait("GetTasksForDay", "Should clamp a multi-day fixed task to the current day")]
    public void GetTasksForDay_ShouldClampMultiDayFixedTaskToTheDay()
    {
        var date = new DateOnly(2026, 6, 24);

        // Spans from 22:00 the previous day to 06:00 the next day. Naively taking TimeOfDay
        // would yield an inverted range (22:00 -> 06:00); it must be clamped to the full day.
        var task = new FixedTask
        {
            UserId = Guid.NewGuid(),
            Name = "Multi-day task",
            Priority = 1,
            StartTimestamp = date.AddDays(-1).ToDateTime(new TimeOnly(22, 0)),
            EndTimestamp = date.AddDays(1).ToDateTime(new TimeOnly(6, 0))
        };

        var result = _processor.GetTasksForDay([task], [], [], date);

        var container = result.TasksTimeline.Single(t => t.IsFixed);
        container.TimeRange.Start.Should().BeLessThan(container.TimeRange.End);
        container.TimeRange.Start.Should().Be(TimeSpan.Zero);
        container.TimeRange.End.Should().Be(new TimeSpan(23, 59, 59));
    }

    [Fact]
    [Trait("GetTasksForDay", "Should not throw when a dynamic task has equal min/max duration")]
    public void GetTasksForDay_ShouldHandleEqualMinMaxDynamicDuration()
    {
        var date = new DateOnly(2026, 6, 24);

        var dynamicTask = new DynamicTask
        {
            UserId = Guid.NewGuid(),
            Name = "Fixed-duration dynamic task",
            Priority = 1,
            MinTimeToFinish = TimeSpan.FromMinutes(30),
            MaxTimeToFinish = TimeSpan.FromMinutes(30)
        };

        var act = () => _processor.GetTasksForDay([], [], [dynamicTask], date);

        act.Should().NotThrow();
    }
}
