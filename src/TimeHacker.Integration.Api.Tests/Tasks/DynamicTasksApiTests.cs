using System.Net;
using AwesomeAssertions;
using TimeHacker.Integration.Api.Tests.Fixtures;

namespace TimeHacker.Integration.Api.Tests.Tasks;

public sealed class DynamicTasksApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Endpoint", "POST+GET /api/dynamic-tasks")]
    public async Task Create_Should_PersistAndRoundTrip()
    {
        var api = await CreateAuthenticatedApiAsync();

        var create = await api.DynamicTasks.Create(TestRequests.NewDynamicTask(
            "Read", min: TimeSpan.FromMinutes(20), max: TimeSpan.FromMinutes(45),
            optimal: TimeSpan.FromMinutes(30), priority: 4));
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
        await api.DynamicTasks.Create(TestRequests.NewDynamicTask("One"));
        await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Two"));

        var all = await api.DynamicTasks.GetAll();

        all.StatusCode.Should().Be(HttpStatusCode.OK);
        all.Content!.Select(t => t.Name).Should().BeEquivalentTo("One", "Two");
    }

    [Fact, Trait("Endpoint", "PUT /api/dynamic-tasks/{id}")]
    public async Task Update_Should_ChangeFields()
    {
        var api = await CreateAuthenticatedApiAsync();
        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Old"))).Content;

        var update = await api.DynamicTasks.Update(id, TestRequests.NewDynamicTask("New", priority: 9));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var get = await api.DynamicTasks.Get(id);
        get.Content!.Name.Should().Be("New");
        get.Content.Priority.Should().Be(9);
    }

    [Fact, Trait("Endpoint", "DELETE /api/dynamic-tasks/{id}")]
    public async Task Delete_Should_Return204_ThenNotFound()
    {
        var api = await CreateAuthenticatedApiAsync();
        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Temp"))).Content;

        (await api.DynamicTasks.Delete(id)).StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await api.DynamicTasks.Get(id)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Create_And_Update_Should_Return400_WhenMinNotLessThanMax()
    {
        var api = await CreateAuthenticatedApiAsync();

        var badCreate = await api.DynamicTasks.Create(TestRequests.NewDynamicTask(
            "Bad", min: TimeSpan.FromMinutes(60), max: TimeSpan.FromMinutes(30)));
        badCreate.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var id = (await api.DynamicTasks.Create(TestRequests.NewDynamicTask("Good"))).Content;
        var badUpdate = await api.DynamicTasks.Update(id, TestRequests.NewDynamicTask(
            "Good", min: TimeSpan.FromMinutes(60), max: TimeSpan.FromMinutes(30)));
        badUpdate.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Get_Update_Delete_Should_Return404_ForUnknownId()
    {
        var api = await CreateAuthenticatedApiAsync();
        var unknown = Guid.CreateVersion7();

        (await api.DynamicTasks.Get(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.DynamicTasks.Update(unknown, TestRequests.NewDynamicTask())).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.DynamicTasks.Delete(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
