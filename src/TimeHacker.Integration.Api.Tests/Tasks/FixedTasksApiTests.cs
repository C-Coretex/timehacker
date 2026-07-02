using System.Net;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Integration.Api.Tests.Fixtures;

namespace TimeHacker.Integration.Api.Tests.Tasks;

public sealed class FixedTasksApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    private static readonly DateTime Start = new(2026, 07, 01, 09, 00, 00, DateTimeKind.Utc);
    private static readonly DateTime End = new(2026, 07, 01, 10, 30, 00, DateTimeKind.Utc);

    [Fact, Trait("Endpoint", "POST+GET /api/fixed-tasks")]
    public async Task Create_Should_PersistAndRoundTripUtcTimestamps()
    {
        var api = await CreateAuthenticatedApiAsync();

        var create = await api.FixedTasks.Create(TestRequests.NewFixedTask("Standup", Start, End, priority: 7));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var get = await api.FixedTasks.Get(create.Content);
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        get.Content!.Name.Should().Be("Standup");
        get.Content.Priority.Should().Be(7);
        get.Content.StartTimestamp.Should().Be(Start);
        get.Content.EndTimestamp.Should().Be(End);
    }

    [Fact, Trait("Endpoint", "POST /api/fixed-tasks")]
    public async Task Create_WithCategoryIds_Should_Succeed()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Cat"))).Content;

        var create = await api.FixedTasks.Create(TestRequests.NewFixedTask(categoryIds: [categoryId]));

        create.StatusCode.Should().Be(HttpStatusCode.Created);
        (await api.FixedTasks.Get(create.Content)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact, Trait("Endpoint", "GET /api/fixed-tasks")]
    public async Task GetAll_Should_StreamOwnedTasks()
    {
        var api = await CreateAuthenticatedApiAsync();
        await api.FixedTasks.Create(TestRequests.NewFixedTask("One"));
        await api.FixedTasks.Create(TestRequests.NewFixedTask("Two"));

        var all = await api.FixedTasks.GetAll();

        all.StatusCode.Should().Be(HttpStatusCode.OK);
        all.Content!.Select(t => t.Name).Should().BeEquivalentTo("One", "Two");
    }

    [Fact, Trait("Endpoint", "PUT /api/fixed-tasks/{id}")]
    public async Task Update_Should_ChangeFields()
    {
        var api = await CreateAuthenticatedApiAsync();
        var id = (await api.FixedTasks.Create(TestRequests.NewFixedTask("Old"))).Content;

        var update = await api.FixedTasks.Update(id, TestRequests.NewFixedTask("New", Start, End, priority: 3));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var get = await api.FixedTasks.Get(id);
        get.Content!.Name.Should().Be("New");
        get.Content.Priority.Should().Be(3);
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Create_And_Update_Should_Return400_WhenStartNotBeforeEnd()
    {
        var api = await CreateAuthenticatedApiAsync();

        var badCreate = await api.FixedTasks.Create(TestRequests.NewFixedTask("Bad", End, Start)); // start after end
        badCreate.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var id = (await api.FixedTasks.Create(TestRequests.NewFixedTask("Good", Start, End))).Content;
        var badUpdate = await api.FixedTasks.Update(id, TestRequests.NewFixedTask("Good", End, Start));
        badUpdate.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Get_Update_Delete_Should_Return404_ForUnknownId()
    {
        var api = await CreateAuthenticatedApiAsync();
        var unknown = Guid.CreateVersion7();

        (await api.FixedTasks.Get(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.FixedTasks.Update(unknown, TestRequests.NewFixedTask())).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.FixedTasks.Delete(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory, Trait("Endpoint", "DELETE cascade")]
    [InlineData(true), InlineData(false)]
    public async Task Delete_Should_CascadeItsScheduleEntity(bool seedScheduledEntities)
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;


        var anotherTaskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask("Recurring", Start, End))).Content;
        var schedule = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(anotherTaskId, TestRequests.EveryNDays(1)));
        schedule.StatusCode.Should().Be(HttpStatusCode.Created);

        var end = End.AddDays(4);
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask("Recurring", Start, End))).Content;
        if(seedScheduledEntities)
        {
            schedule = await api.Tasks.CreateSchedule(TestRequests.NewSchedule(taskId, TestRequests.EveryNDays(1)));
            schedule.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        (await AdminDbContext.Set<FixedTask>().CountAsync(cancellationToken)).Should().Be(2);
        (await AdminDbContext.Set<ScheduleEntity>().CountAsync(cancellationToken)).Should().Be(seedScheduledEntities ? 2 : 1);

        var delete = await api.FixedTasks.Delete(taskId);
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await AdminDbContext.Set<FixedTask>().CountAsync(cancellationToken)).Should().Be(1);
        (await AdminDbContext.Set<ScheduleEntity>().CountAsync(cancellationToken)).Should().Be(1);
    }
}
