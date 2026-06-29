using Microsoft.EntityFrameworkCore;
using Timehacker.Integration.Db.Tests;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Infrastructure;
using TimeHacker.Integration.Db.Tests.Fixtures;

namespace TimeHacker.Integration.Db.Tests.ServiceFlowTests;

// End-to-end TaskService orchestration against the database: snapshot generation,
// gap-fill, refresh ordering, and recurrence expansion.
public class TaskServiceSnapshotTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("GetTasksForDay", "SnapshotMissGeneratesSnapshot")]
    public async Task GetTasksForDay_OnSnapshotMiss_Should_GenerateAndPersistSnapshot()
    {
        var date = new DateOnly(2026, 6, 10);
        await SeedFixedTaskOn(date, "Morning");

        var result = await Resolve<ITaskAppService>().GetTasksForDay(date, TestContext.Current.CancellationToken);

        result.Date.Should().Be(date);
        result.TasksTimeline.Should().Contain(t => t.Task.Name == "Morning");

        var snapshotCount = await CountSnapshotsOn(date);
        snapshotCount.Should().Be(1);
    }

    [Fact]
    [Trait("GetTasksForDay", "SnapshotHitReusesSnapshot")]
    public async Task GetTasksForDay_OnSecondCall_Should_ReuseSnapshot()
    {
        var date = new DateOnly(2026, 6, 10);
        await SeedFixedTaskOn(date, "Morning");
        var service = Resolve<ITaskAppService>();

        await service.GetTasksForDay(date, TestContext.Current.CancellationToken);
        await service.GetTasksForDay(date, TestContext.Current.CancellationToken);

        (await CountSnapshotsOn(date)).Should().Be(1);
    }

    [Fact]
    [Trait("GetTasksForDays", "GapFill")]
    public async Task GetTasksForDays_Should_FillMissingDatesExactlyOnce()
    {
        var dates = new List<DateOnly> { new(2026, 6, 10), new(2026, 6, 11), new(2026, 6, 12) };
        var service = Resolve<ITaskAppService>();

        // Pre-create the middle date's snapshot so the call has a mix of present + missing.
        await service.GetTasksForDay(dates[1], TestContext.Current.CancellationToken);

        var results = await service.GetTasksForDays(dates, TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        results.Should().HaveCount(3);
        Db.ChangeTracker.Clear();
        var snapshotCount = await Db.Set<ScheduleSnapshot>().CountAsync(s => dates.Contains(s.Date), TestContext.Current.CancellationToken);
        snapshotCount.Should().Be(3);
    }

    [Fact]
    [Trait("RefreshTasksForDays", "ReplacesWithoutDuplicating")]
    public async Task RefreshTasksForDays_Should_ReplaceSnapshotsWithoutDuplicating()
    {
        var date = new DateOnly(2026, 6, 10);
        var dates = new List<DateOnly> { date };
        await SeedFixedTaskOn(date, "Morning");
        var service = Resolve<ITaskAppService>();

        await service.GetTasksForDays(dates, TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);
        // Production runs each request in its own DI scope/DbContext; emulate that so the refresh doesn't
        // collide with snapshots still tracked from the previous call. The service uses the current
        // user's scoped context (not the admin Db), so clear that one.
        Resolve<TimeHackerDbContext>().ChangeTracker.Clear();
        await service.RefreshTasksForDays(dates, TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

        // Exactly one snapshot for the date, and its single child task was replaced, not duplicated.
        (await CountSnapshotsOn(date)).Should().Be(1);
        var scheduledTaskCount = await Db.Set<ScheduledTask>().CountAsync(t => t.Date == date, TestContext.Current.CancellationToken);
        scheduledTaskCount.Should().Be(1);
    }

    [Theory]
    [InlineData(1), InlineData(3)]
    [Trait("RefreshTasksForDays", "ExpandsRecurrence")]
    public async Task RefreshTasksForDays_Should_ExpandRecurrenceAndTrackLastEntityCreated(int numberOfDays)
    {
        var task = await Resolve<GraphSeeder>().SeedFixedTaskWithSchedule(TestContext.Current.CancellationToken);
        var scheduleEntityId = task.ScheduleEntityId!.Value;
        // A date a few days out: the daily recurrence (anchored at the schedule's creation = now) will hit it.
        var date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3);

        // Start from a clean tracker, as a fresh request scope would.
        Db.ChangeTracker.Clear();
        var results = await Resolve<ITaskAppService>()
            .RefreshTasksForDays(Enumerable.Range(0, numberOfDays).Select(date.AddDays).ToList(), TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        results.Count.Should().Be(numberOfDays);
        results.All(r => r.TasksTimeline.Any(t => t.Task.Name == task.Name)).Should().BeTrue();

        Db.ChangeTracker.Clear();
        var scheduleEntity = await Db.Set<ScheduleEntity>().FirstAsync(x => x.Id == scheduleEntityId, TestContext.Current.CancellationToken);
        scheduleEntity.LastEntityCreated.Should().Be(date.AddDays(numberOfDays - 1));
        scheduleEntity.FirstEntityCreated.Should().Be(date);
    }

    private Task SeedFixedTaskOn(DateOnly date, string name)
        => Resolve<GraphSeeder>().SeedFixedTaskOn(date, name, TestContext.Current.CancellationToken);

    private async Task<int> CountSnapshotsOn(DateOnly date)
    {
        Db.ChangeTracker.Clear();
        return await Db.Set<ScheduleSnapshot>().CountAsync(s => s.Date == date, TestContext.Current.CancellationToken);
    }
}
