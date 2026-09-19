using System.Text.Json;
using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Integration.Api.Tests.Tasks;

public sealed class DynamicTasksApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Endpoint", "POST+GET /api/dynamic-tasks")]
    public async Task Create_Should_PersistAndRoundTrip()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = await NewCategoryId(api);

        var create = await api.DynamicTasks.Create(TestRequests.NewDynamicTask(
            "Read", min: TimeSpan.FromMinutes(20), max: TimeSpan.FromMinutes(45),
            optimal: TimeSpan.FromMinutes(30), priority: 4, categoryIds: [categoryId]));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var get = await api.DynamicTasks.Get(create.Content);
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        get.Content!.Name.Should().Be("Read");
        get.Content.Priority.Should().Be(4);
        get.Content.MinTimeToFinish.Should().Be(TimeSpan.FromMinutes(20));
        get.Content.MaxTimeToFinish.Should().Be(TimeSpan.FromMinutes(45));
        get.Content.OptimalTimeToFinish.Should().Be(TimeSpan.FromMinutes(30));
    }

    [Fact, Trait("Endpoint", "GET /api/dynamic-tasks")]
    public async Task GetAll_Should_StreamOwnedTasks()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = await NewCategoryId(api);
        await api.DynamicTasks.Create(TestRequests.NewDynamicTask("One", categoryIds: [categoryId]));
        await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Two", categoryIds: [categoryId]));

        var all = await api.DynamicTasks.GetAll();

        all.StatusCode.Should().Be(HttpStatusCode.OK);
        all.Content!.Select(t => t.Name).Should().BeEquivalentTo("One", "Two");
    }

    [Fact, Trait("Endpoint", "PUT /api/dynamic-tasks/{id}")]
    public async Task Update_Should_ChangeFields()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = await NewCategoryId(api);
        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Old", categoryIds: [categoryId]))).Content;

        var update = await api.DynamicTasks.Update(id, TestRequests.NewDynamicTask("New", priority: 9, categoryIds: [categoryId]));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var get = await api.DynamicTasks.Get(id);
        get.Content!.Name.Should().Be("New");
        get.Content.Priority.Should().Be(9);
    }

    [Fact, Trait("Endpoint", "DELETE /api/dynamic-tasks/{id}")]
    public async Task Delete_Should_Return204_ThenNotFound()
    {
        var api = await CreateAuthenticatedApiAsync();
        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Temp", categoryIds: [await NewCategoryId(api)]))).Content;

        (await api.DynamicTasks.Delete(id)).StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await api.DynamicTasks.Get(id)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Create_And_Update_Should_Return400_WhenMinNotLessThanMax()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = await NewCategoryId(api);

        var badCreate = await api.DynamicTasks.Create(TestRequests.NewDynamicTask(
            "Bad", min: TimeSpan.FromMinutes(60), max: TimeSpan.FromMinutes(30), categoryIds: [categoryId]));
        badCreate.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Good", categoryIds: [categoryId]))).Content;
        var badUpdate = await api.DynamicTasks.Update(id, TestRequests.NewDynamicTask(
            "Good", min: TimeSpan.FromMinutes(60), max: TimeSpan.FromMinutes(30), categoryIds: [categoryId]));
        badUpdate.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Endpoint", "POST /api/dynamic-tasks")]
    public async Task Create_WithCategoryIds_Should_LinkAndRoundTripThem()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Cat"))).Content;

        var create = await api.DynamicTasks.Create(TestRequests.NewDynamicTask(categoryIds: [categoryId]));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        (await LinkedCategoryIdsOf(create.Content)).Should().ContainSingle(id => id == categoryId);

        // The link must also come back out of both read endpoints, named, not just id-shaped.
        var get = await api.DynamicTasks.Get(create.Content);
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        get.Content!.Categories.Should().ContainSingle(c => c.Id == categoryId && c.Name == "Cat");

        var all = await api.DynamicTasks.GetAll();
        all.Content!.Single().Categories.Should().ContainSingle(c => c.Id == categoryId && c.Name == "Cat");
    }

    [Fact, Trait("Endpoint", "PUT /api/dynamic-tasks/{id}")]
    public async Task Update_Should_ReplaceLinkedCategories()
    {
        var api = await CreateAuthenticatedApiAsync();
        var first = (await api.Categories.Create(TestRequests.NewCategory("First"))).Content;
        var second = (await api.Categories.Create(TestRequests.NewCategory("Second"))).Content;
        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask(categoryIds: [first]))).Content;

        var update = await api.DynamicTasks.Update(id, TestRequests.NewDynamicTask(categoryIds: [second]));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        (await LinkedCategoryIdsOf(id)).Should().BeEquivalentTo([second]);
        (await api.DynamicTasks.Get(id)).Content!.Categories.Should().ContainSingle(c => c.Id == second);
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Create_And_Update_Should_Return400_WithoutCategoryIds()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = await NewCategoryId(api);

        // A dynamic task must always carry at least one category — a fixed task need not.
        await AssertCategoryRequired(await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Uncategorised")));

        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask(categoryIds: [categoryId]))).Content;
        await AssertCategoryRequired(await api.DynamicTasks.Update(id, TestRequests.NewDynamicTask()));

        // The rejected update left the existing link in place.
        (await LinkedCategoryIdsOf(id)).Should().BeEquivalentTo([categoryId]);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Create_And_Update_Should_Return404_ForUnknownCategoryId()
    {
        var api = await CreateAuthenticatedApiAsync();
        var unknownCategory = Guid.CreateVersion7();

        await AssertCategoryNotFound(await api.DynamicTasks.Create(TestRequests.NewDynamicTask(categoryIds: [unknownCategory])));

        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask())).Content;
        await AssertCategoryNotFound(await api.DynamicTasks.Update(id, TestRequests.NewDynamicTask(categoryIds: [unknownCategory])));
        (await LinkedCategoryIdsOf(id)).Should().BeEmpty();
    }

    [Fact, Trait("Endpoint", "Cross-user 404")]
    public async Task Create_Should_Return404_ForAnotherUsersCategory()
    {
        var userA = await CreateAuthenticatedApiAsync();
        var categoryId = (await userA.Categories.Create(TestRequests.NewCategory("A-only"))).Content;

        // RLS hides A's category from B on reads, and a foreign-key check would not: the link must be refused.
        var userB = await CreateAuthenticatedApiAsync();
        await AssertCategoryNotFound(await userB.DynamicTasks.Create(TestRequests.NewDynamicTask(categoryIds: [categoryId])));

        (await AdminDbContext.Set<CategoryDynamicTask>().CountAsync(TestContext.Current.CancellationToken)).Should().Be(0);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Get_Update_Delete_Should_Return404_ForUnknownId()
    {
        var api = await CreateAuthenticatedApiAsync();
        var unknown = Guid.CreateVersion7();
        var categoryId = await NewCategoryId(api);

        (await api.DynamicTasks.Get(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.DynamicTasks.Update(unknown, TestRequests.NewDynamicTask(categoryIds: [categoryId]))).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.DynamicTasks.Delete(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Trait("Endpoint", "GET /api/dynamic-tasks")]
    public async Task Get_Should_ReportEachCategoryWithItsWindows()
    {
        var api = await CreateAuthenticatedApiAsync();
        var categoryId = (await api.Categories.Create(TestRequests.NewCategory("Cat"))).Content;
        var windowId = (await api.Categories.CreateSchedule(categoryId, TestRequests.NewCategorySchedule())).Content;
        var taskId = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask(categoryIds: [categoryId]))).Content;

        // Both read paths build the DTO differently — one an EF projection, one a compiled selector over an
        // Included graph — so both are held to carrying the category's windows.
        (await api.DynamicTasks.Get(taskId)).Content!.Categories.Single()
            .Schedules.Should().ContainSingle(s => s.Id == windowId);
        (await api.DynamicTasks.GetAll()).Content!.Single().Categories.Single()
            .Schedules.Should().ContainSingle(s => s.Id == windowId);
    }

    private static async Task<Guid> NewCategoryId(TimeHackerApi api)
        => (await api.Categories.Create(TestRequests.NewCategory("Cat"))).Content;

    private static async Task AssertCategoryRequired(IApiResponse response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var body = JsonDocument.Parse(((ApiException)response.Error!).Content!);
        body.RootElement.GetProperty("title").GetString().Should().Be("Parameter is not correct.");
    }

    private async Task<List<Guid>> LinkedCategoryIdsOf(Guid taskId)
        => await AdminDbContext.Set<CategoryDynamicTask>()
            .Where(x => x.DynamicTaskId == taskId)
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
