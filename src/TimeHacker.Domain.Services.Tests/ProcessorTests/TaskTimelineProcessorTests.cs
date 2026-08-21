using System.Drawing;
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

        var result = _processor.GetTasksForDay([task], [], [], [], date);

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

        var act = () => _processor.GetTasksForDay([], [], [dynamicTask], [], date);

        act.Should().NotThrow();
    }

    private static Category NewCategory(string name, TimeOnly start, TimeOnly end) => new()
    {
        UserId = Guid.NewGuid(),
        Name = name,
        Color = Color.Blue,
        StartTime = start,
        EndTime = end,
        ScheduleEntityId = Guid.NewGuid()
    };

    [Fact]
    [Trait("GetTasksForDay", "Should map categories onto the day's time windows")]
    public void GetTasksForDay_ShouldMapCategoriesToTimeWindows()
    {
        var date = new DateOnly(2026, 6, 24);
        var category = NewCategory("Work", new TimeOnly(09, 00), new TimeOnly(18, 00));

        var result = _processor.GetTasksForDay([], [], [], [category], date);

        var container = result.CategoriesTimeline.Single();
        container.Category.Name.Should().Be("Work");
        container.ScheduleEntityId.Should().Be(category.ScheduleEntityId);
        container.TimeRange.Start.Should().Be(new TimeSpan(09, 00, 00));
        container.TimeRange.End.Should().Be(new TimeSpan(18, 00, 00));
    }

    [Fact]
    [Trait("GetTasksForDay", "Should keep every overlapping category")]
    public void GetTasksForDay_ShouldKeepOverlappingCategories()
    {
        var date = new DateOnly(2026, 6, 24);

        // Categories are a backdrop, not a schedule — several may cover the same hour and all must survive.
        var work = NewCategory("Work", new TimeOnly(09, 00), new TimeOnly(18, 00));
        var meetings = NewCategory("Meetings", new TimeOnly(12, 00), new TimeOnly(14, 00));

        var result = _processor.GetTasksForDay([], [], [], [work, meetings], date);

        result.CategoriesTimeline.Should().HaveCount(2);
        result.CategoriesTimeline.Select(c => c.Category.Name).Should().Equal("Work", "Meetings");
    }

    [Fact]
    [Trait("GetTasksForDay", "Categories should not consume time from dynamic task placement")]
    public void GetTasksForDay_CategoriesShouldNotAffectTaskPlacement()
    {
        var date = new DateOnly(2026, 6, 24);
        var userId = Guid.NewGuid();

        var fixedTask = new FixedTask
        {
            UserId = userId,
            Name = "Standup",
            Priority = 1,
            StartTimestamp = date.ToDateTime(new TimeOnly(09, 0)),
            EndTimestamp = date.ToDateTime(new TimeOnly(10, 0))
        };

        var withoutCategories = _processor.GetTasksForDay([fixedTask], [], [], [], date);
        var withCategories = _processor.GetTasksForDay([fixedTask], [], [], [NewCategory("Work", new TimeOnly(09, 00), new TimeOnly(18, 00))], date);

        withCategories.TasksTimeline.Select(t => t.TimeRange)
            .Should().Equal(withoutCategories.TasksTimeline.Select(t => t.TimeRange));
    }
}
