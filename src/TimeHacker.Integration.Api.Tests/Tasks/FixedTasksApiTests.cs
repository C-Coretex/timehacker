using System.Text.Json;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Integration.Api.Tests.Tasks;

public sealed class FixedTasksApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    private static readonly DateTime Start = new(2026, 07, 01, 09, 00, 00, DateTimeKind.Utc);
    private static readonly DateTime End = new(2026, 07, 01, 10, 30, 00, DateTimeKind.Utc);

    [Fact, Trait("Endpoint", "POST+GET /api/fixed-tasks")]
    public async Task Create_Should_PersistAndRoundTripUtcTimestamps()
    {
        var api = await CreateAuthenticatedApiAsync();

        var create = await api.FixedTasks.Create(TestRequests.NewFixedTask("Standup", Start, End, priority: 2));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var get = await api.FixedTasks.Get(create.Content);
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        get.Content!.Name.Should().Be("Standup");
        get.Content.Priority.Should().Be(2);
        get.Content.StartTimestamp.Should().Be(Start);
        get.Content.EndTimestamp.Should().Be(End);
    }

    [Fact, Trait("Endpoint", "POST /api/fixed-tasks")]
    public async Task Create_WithCategoryIds_Should_LinkAndRoundTripThem()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Cat"))).Content;

        var create = await api.FixedTasks.Create(TestRequests.NewFixedTask(categoryIds: [categoryId]));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        (await LinkedCategoryIdsOf(create.Content)).Should().ContainSingle(id => id == categoryId);

        // The link must also come back out of both read endpoints, named, not just id-shaped.
        var get = await api.FixedTasks.Get(create.Content);
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        get.Content!.Categories.Should().ContainSingle(c => c.Id == categoryId && c.Name == "Cat");

        var all = await api.FixedTasks.GetAll();
        all.Content!.Single().Categories.Should().ContainSingle(c => c.Id == categoryId && c.Name == "Cat");
    }

    [Fact, Trait("Endpoint", "PUT /api/fixed-tasks/{id}")]
    public async Task Update_Should_ReplaceLinkedCategories()
    {
        var api = await CreateAuthenticatedApiAsync();
        var first = (await api.Categories.Create(TestRequests.NewCategory("First"))).Content;
        var second = (await api.Categories.Create(TestRequests.NewCategory("Second"))).Content;
        var id = (await api.FixedTasks.Create(TestRequests.NewFixedTask(categoryIds: [first]))).Content;

        var update = await api.FixedTasks.Update(id, TestRequests.NewFixedTask(categoryIds: [second]));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        (await LinkedCategoryIdsOf(id)).Should().BeEquivalentTo([second]);
        (await api.FixedTasks.Get(id)).Content!.Categories.Should().ContainSingle(c => c.Id == second);
    }

    [Fact, Trait("Endpoint", "PUT /api/fixed-tasks/{id}")]
    public async Task Update_WithoutCategoryIds_Should_UnlinkAll()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Cat"))).Content;
        var id = (await api.FixedTasks.Create(TestRequests.NewFixedTask(categoryIds: [categoryId]))).Content;

        (await api.FixedTasks.Update(id, TestRequests.NewFixedTask())).StatusCode.Should().Be(HttpStatusCode.OK);

        (await LinkedCategoryIdsOf(id)).Should().BeEmpty();
        // Unlinking must not take the category with it.
        (await api.Categories.Get(categoryId)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Create_And_Update_Should_Return404_ForUnknownCategoryId()
    {
        var api = await CreateAuthenticatedApiAsync();
        var unknownCategory = Guid.CreateVersion7();

        var create = await api.FixedTasks.Create(TestRequests.NewFixedTask(categoryIds: [unknownCategory]));
        await AssertCategoryNotFound(create);

        var id = (await api.FixedTasks.Create(TestRequests.NewFixedTask())).Content;
        await AssertCategoryNotFound(await api.FixedTasks.Update(id, TestRequests.NewFixedTask(categoryIds: [unknownCategory])));
        (await LinkedCategoryIdsOf(id)).Should().BeEmpty();
    }

    [Fact, Trait("Endpoint", "Cross-user 404")]
    public async Task Create_Should_Return404_ForAnotherUsersCategory()
    {
        var userA = await CreateAuthenticatedApiAsync();
        var categoryId = (await userA.Categories.Create(TestRequests.NewCategory("A-only"))).Content;

        // RLS hides A's category from B on reads, and a foreign-key check would not: the link must be refused.
        var userB = await CreateAuthenticatedApiAsync();
        await AssertCategoryNotFound(await userB.FixedTasks.Create(TestRequests.NewFixedTask(categoryIds: [categoryId])));

        (await AdminDbContext.Set<CategoryFixedTask>().CountAsync(TestContext.Current.CancellationToken)).Should().Be(0);
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

    [Theory, Trait("Endpoint", "Validation")]
    [InlineData(0)]
    [InlineData(6)]
    public async Task Create_And_Update_Should_Return400_WhenPriorityOutOfRange(byte priority)
    {
        var api = await CreateAuthenticatedApiAsync();

        (await api.FixedTasks.Create(TestRequests.NewFixedTask(priority: priority))).ShouldBeValidationErrorFor("Priority");

        var id = (await api.FixedTasks.Create(TestRequests.NewFixedTask())).Content;
        (await api.FixedTasks.Update(id, TestRequests.NewFixedTask(priority: priority))).ShouldBeValidationErrorFor("Priority");
        (await api.FixedTasks.Get(id)).Content!.Priority.Should().Be(PriorityConstants.Default);
    }

    [Theory, Trait("Endpoint", "Validation")]
    [InlineData(PriorityConstants.Highest)]
    [InlineData(PriorityConstants.Lowest)]
    public async Task Create_Should_AcceptPriorityBounds(byte priority)
    {
        var api = await CreateAuthenticatedApiAsync();

        var create = await api.FixedTasks.Create(TestRequests.NewFixedTask(priority: priority));

        create.StatusCode.Should().Be(HttpStatusCode.Created);
        (await api.FixedTasks.Get(create.Content)).Content!.Priority.Should().Be(priority);
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

    [Fact, Trait("Endpoint", "GET /api/fixed-tasks")]
    public async Task Get_Should_ReportEachCategoryWithItsWindows()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Cat"))).Content;
        var windowId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule())).Content;
        var taskId = (await api.FixedTasks.Create(TestRequests.NewFixedTask(categoryIds: [categoryId]))).Content;

        // Both read paths build the DTO differently — one an EF projection, one a compiled selector over an
        // Included graph — so both are held to carrying the category's windows.
        (await api.FixedTasks.Get(taskId)).Content!.Categories.Single()
            .Schedules.Should().ContainSingle(s => s.Id == windowId);
        (await api.FixedTasks.GetAll()).Content!.Single().Categories.Single()
            .Schedules.Should().ContainSingle(s => s.Id == windowId);
    }

    private async Task<List<Guid>> LinkedCategoryIdsOf(Guid taskId)
        => await AdminDbContext.Set<CategoryFixedTask>()
            .Where(x => x.FixedTaskId == taskId)
            .Select(x => x.CategoryId)
            .ToListAsync(TestContext.Current.CancellationToken);

    private static async Task AssertCategoryNotFound(IApiResponse response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("Resource is not found.");
        body.RootElement.GetProperty("ResourceName").GetString().Should().Be("Category");
    }
}
