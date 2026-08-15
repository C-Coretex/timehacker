using System.Globalization;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Integration.Api.Tests.Tasks;

public sealed class TimelineApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    private static string D(DateOnly date) => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static (DateTime Start, DateTime End) SlotOn(DateOnly date)
        => (date.ToDateTime(new TimeOnly(09, 00)), date.ToDateTime(new TimeOnly(10, 00)));

    // Read the snapshot fresh (no tracking) so its Id/CreatedTimestamp/UpdatedTimestamp reflect the DB row and
    // can be compared across calls to prove insert / delete+reinsert / untouched / updated.
    private Task<ScheduleSnapshot> SnapshotOn(DateOnly date, CancellationToken cancellationToken)
        => AdminDbContext.Set<ScheduleSnapshot>().AsNoTracking().SingleAsync(s => s.Date == date, cancellationToken);

    [Fact, Trait("Endpoint", "GET /api/tasks/timeline/day")]
    public async Task GetTasksForDay_Should_PlaceFixedTask_AndPersistSnapshot()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var (start, end) = SlotOn(Today);
        await api.FixedTasks.Create(TestRequests.NewFixedTask("Morning", start, end));

        var day = await api.Tasks.GetForDay(D(Today));

        day.StatusCode.Should().Be(HttpStatusCode.OK);
        day.Content!.Date.Should().Be(Today);
        day.Content.TasksTimeline.Should().Contain(t => t.Task.Name == "Morning");
        (await AdminDbContext.Set<ScheduleSnapshot>().CountAsync(s => s.Date == Today, cancellationToken)).Should().Be(1);
    }

    [Fact, Trait("Endpoint", "GET /api/tasks/timeline/day")]
    public async Task GetTasksForDay_SecondCall_Should_ReuseSnapshot()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var (start, end) = SlotOn(Today);
        await api.FixedTasks.Create(TestRequests.NewFixedTask("Morning", start, end));

        await api.Tasks.GetForDay(D(Today));
        var afterFirst = await SnapshotOn(Today, cancellationToken);
        // Let the clock advance so that IF the second call regenerated the snapshot its CreatedTimestamp would
        // differ — making the "unchanged timestamp" assertion below a real proof that the row was reused.
        await Task.Delay(10, cancellationToken);

        await api.Tasks.GetForDay(D(Today));
        var afterSecond = await SnapshotOn(Today, cancellationToken);

        (await AdminDbContext.Set<ScheduleSnapshot>().CountAsync(s => s.Date == Today, cancellationToken)).Should().Be(1);
        // Reused, not touched: the same row survives (Id + CreatedTimestamp unchanged) and was never updated.
        afterSecond.Id.Should().Be(afterFirst.Id);
        afterSecond.CreatedTimestamp.Should().Be(afterFirst.CreatedTimestamp);
        afterSecond.UpdatedTimestamp.Should().BeNull();
    }

    [Fact, Trait("Endpoint", "GET /api/tasks/timeline")]
    public async Task GetTasksForDays_Should_ReturnOneEntryPerDate()
    {
        var api = await CreateAuthenticatedApiAsync();
        var dates = new[] { Today, Today.AddDays(1), Today.AddDays(2) };

        var response = await api.Tasks.GetForDays(dates.Select(D));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content!.Select(d => d.Date).Should().BeEquivalentTo(dates);
    }

    [Fact, Trait("Endpoint", "POST /api/tasks/timeline/refresh")]
    public async Task RefreshTasksForDays_Should_RegenerateWithoutDuplicating()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var (start, end) = SlotOn(Today);
        await api.FixedTasks.Create(TestRequests.NewFixedTask("Morning", start, end));

        await api.Tasks.GetForDay(D(Today)); // seed a snapshot first
        var original = await SnapshotOn(Today, cancellationToken);
        // Guarantee the regenerated snapshot's CreatedTimestamp is strictly later than the original's so the
        // "deleted and re-inserted" assertion below can't be satisfied by an unchanged row sharing a timestamp.
        await Task.Delay(10, cancellationToken);

        var refreshed = await api.Tasks.RefreshForDays([Today]);

        refreshed.StatusCode.Should().Be(HttpStatusCode.OK);
        refreshed.Content!.Should().HaveCount(1);
        (await AdminDbContext.Set<ScheduleSnapshot>().CountAsync(s => s.Date == Today, cancellationToken)).Should().Be(1);
        // Regenerated: the old snapshot was deleted and a new one inserted (new Id, strictly newer
        // CreatedTimestamp), not touched in place and not duplicated.
        var regenerated = await SnapshotOn(Today, cancellationToken);
        regenerated.Id.Should().NotBe(original.Id);
        regenerated.CreatedTimestamp.Should().BeAfter(original.CreatedTimestamp);
        regenerated.UpdatedTimestamp.Should().BeNull();
    }

    [Theory, Trait("Endpoint", "Validation")]
    [InlineData("not-a-date")]
    [InlineData("2026-13-40")]
    [InlineData("07/01/2026")]
    public async Task GetTasksForDay_Should_Return400_ForBadDateFormat(string badDate)
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Tasks.GetForDay(badDate);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Endpoint", "Recurrence")]
    public async Task GetTasksForDays_Should_ExpandDailyRecurrenceIntoFutureDays()
    {
        var api = await CreateAuthenticatedApiAsync();
        var (start, end) = SlotOn(Today);
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask("Recurring", start, end))).Content;
        await api.Tasks.CreateSchedule(TestRequests.NewSchedule(taskId, TestRequests.EveryNDays(1)));

        var future = Today.AddDays(3);
        var response = await api.Tasks.GetForDays([D(future)]);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content!.Single().TasksTimeline.Should().Contain(t => t.Task.Name == "Recurring");
    }

    [Fact, Trait("Endpoint", "GET /api/tasks/scheduled/{id}")]
    public async Task GetScheduledTask_Should_ReturnGeneratedInstance()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var (start, end) = SlotOn(Today);
        await api.FixedTasks.Create(TestRequests.NewFixedTask("Morning", start, end));
        await api.Tasks.GetForDay(D(Today)); // generates the snapshot + scheduled task rows

        var scheduled = await AdminDbContext.Set<ScheduledTask>().AsNoTracking().FirstAsync(cancellationToken);
        var response = await api.Tasks.GetScheduled(scheduled.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content!.Name.Should().Be("Morning");
    }
}
