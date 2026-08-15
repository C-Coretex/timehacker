using TimeHacker.Api.Models.Return.RepeatingEntities;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
using DomainDayOfWeek = TimeHacker.Domain.Models.EntityModels.Enums.DayOfWeek;

namespace TimeHacker.Integration.Api.Tests.Tasks;

public sealed class SchedulesApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Endpoint", "POST /api/tasks/schedules")]
    public async Task Create_Day_Should_LinkScheduleToFixedTask()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask())).Content;

        var response = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(taskId, TestRequests.EveryNDays(3)));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var day = response.Content!.RepeatingEntity.Should().BeOfType<ReturnDayRepeatingEntityModel>().Subject;
        day.DaysCountToRepeat.Should().Be(3);

        // The parent fixed task now points at the created schedule entity.
        var task = await AdminDbContext.Set<FixedTask>().SingleAsync(cancellationToken);
        task.ScheduleEntityId.Should().Be(response.Content.Id);
    }

    [Fact, Trait("Endpoint", "POST /api/tasks/schedules")]
    public async Task Create_Week_Should_RoundTripSelectedDays()
    {
        var api = await CreateAuthenticatedApiAsync();
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask())).Content;

        var response = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(
            taskId, TestRequests.EveryWeekOn(DomainDayOfWeek.Monday, DomainDayOfWeek.Friday)));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var week = response.Content!.RepeatingEntity.Should().BeOfType<ReturnWeekRepeatingEntityModel>().Subject;
        week.RepeatsOn.Should().BeEquivalentTo([DomainDayOfWeek.Monday, DomainDayOfWeek.Friday]);
    }

    [Fact, Trait("Endpoint", "POST /api/tasks/schedules")]
    public async Task Create_Month_Should_RoundTripDay()
    {
        var api = await CreateAuthenticatedApiAsync();
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask())).Content;

        var response = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(taskId, TestRequests.EveryMonthOnDay(15)));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Content!.RepeatingEntity.Should().BeOfType<ReturnMonthRepeatingEntityModel>()
            .Which.MonthDayToRepeat.Should().Be(15);
    }

    [Fact, Trait("Endpoint", "POST /api/tasks/schedules")]
    public async Task Create_Year_Should_RoundTripDay()
    {
        var api = await CreateAuthenticatedApiAsync();
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask())).Content;

        var response = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(taskId, TestRequests.EveryYearOnDay(200)));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Content!.RepeatingEntity.Should().BeOfType<ReturnYearRepeatingEntityModel>()
            .Which.YearDayToRepeat.Should().Be(200);
    }

    [Fact, Trait("Endpoint", "POST /api/tasks/schedules")]
    public async Task Create_WithEndsOnMaxDate_Should_PopulateEndsOn()
    {
        var api = await CreateAuthenticatedApiAsync();
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask())).Content;
        var endsOn = new DateOnly(2027, 01, 01);

        var response = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(
            taskId, TestRequests.EveryNDays(1), new EndsOnModel { MaxDate = endsOn }));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Content!.EndsOn.Should().Be(endsOn);
    }

    [Fact, Trait("Endpoint", "POST /api/tasks/schedules")]
    public async Task Create_WithEndsOnMaxOccurrences_Should_PopulateEndsOn()
    {
        var api = await CreateAuthenticatedApiAsync();
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask())).Content;

        var response = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(
            taskId, TestRequests.EveryNDays(1), new EndsOnModel { MaxOccurrences = 5 }));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Content!.EndsOn.Should().NotBeNull();
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Create_Should_Return404_ForUnknownParent()
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(Guid.CreateVersion7(), TestRequests.EveryNDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task Create_Should_Return404_ForAnotherUsersTask()
    {
        var userA = await CreateAuthenticatedApiAsync();
        var taskId = (await userA.FixedTasks.Create(TestRequests.NewFixedTask())).Content;

        var userB = await CreateAuthenticatedApiAsync();
        var response = await userB.Tasks.CreateSchedule(TestRequests.NewSchedule(taskId, TestRequests.EveryNDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Trait("Endpoint", "GET /api/tasks/scheduled/{id}")]
    public async Task GetScheduledTask_Should_Return404_ForUnknownId()
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Tasks.GetScheduled(Guid.CreateVersion7());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
