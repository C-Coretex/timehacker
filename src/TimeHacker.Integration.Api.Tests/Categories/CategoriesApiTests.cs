using System.Drawing;
using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Integration.Api.Tests.Categories;

public sealed class CategoriesApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Endpoint", "POST+GET /api/categories")]
    public async Task Create_Should_PersistAndRoundTrip()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        var create = await api.Categories.Create(TestRequests.NewCategory("Work", Color.Teal, "desc"));
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var get = await api.Categories.Get(create.Content);
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        get.Content!.Name.Should().Be("Work");
        get.Content.Description.Should().Be("desc");
        get.Content.Color.ToArgb().Should().Be(Color.Teal.ToArgb());

        (await AdminDbContext.Set<Category>().CountAsync(cancellationToken)).Should().Be(1);
    }

    [Fact, Trait("Endpoint", "GET /api/categories")]
    public async Task GetAll_Should_StreamAllOwnedCategories()
    {
        var api = await CreateAuthenticatedApiAsync();

        await api.Categories.Create(TestRequests.NewCategory("A"));
        await api.Categories.Create(TestRequests.NewCategory("B"));
        await api.Categories.Create(TestRequests.NewCategory("C"));

        var all = await api.Categories.GetAll();
        all.StatusCode.Should().Be(HttpStatusCode.OK);
        all.Content!.Select(c => c.Name).Should().BeEquivalentTo("A", "B", "C");
    }

    [Fact, Trait("Endpoint", "PUT /api/categories/{id}")]
    public async Task Update_Should_ChangeNameAndColor()
    {
        var api = await CreateAuthenticatedApiAsync();

        var id = (await api.Categories.Create(TestRequests.NewCategory("Old", Color.Red))).Content;

        var update = await api.Categories.Update(id, TestRequests.NewCategory("New", Color.Green, "updated"));
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var get = await api.Categories.Get(id);
        get.Content!.Name.Should().Be("New");
        get.Content.Description.Should().Be("updated");
        get.Content.Color.ToArgb().Should().Be(Color.Green.ToArgb());
    }

    [Fact, Trait("Endpoint", "DELETE /api/categories/{id}")]
    public async Task Delete_Should_Return204_AndRemoveRow()
    {
        var api = await CreateAuthenticatedApiAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        var id = (await api.Categories.Create(TestRequests.NewCategory("Temp"))).Content;

        var delete = await api.Categories.Delete(id);
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await api.Categories.Get(id)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await AdminDbContext.Set<Category>().CountAsync(cancellationToken)).Should().Be(0);
    }

    [Fact, Trait("Endpoint", "Not found")]
    public async Task Get_Update_Delete_Should_Return404_ForUnknownId()
    {
        var api = await CreateAuthenticatedApiAsync();
        var unknown = Guid.CreateVersion7();

        (await api.Categories.Get(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.Categories.Update(unknown, TestRequests.NewCategory())).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await api.Categories.Delete(unknown)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Trait("Endpoint", "Validation")]
    public async Task Create_Should_Return400_WhenNameEmpty()
    {
        var api = await CreateAuthenticatedApiAsync();

        var response = await api.Categories.Create(TestRequests.NewCategory(name: ""));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task Get_ByAnotherUser_Should_Return404()
    {
        var userA = await CreateAuthenticatedApiAsync();
        var id = (await userA.Categories.Create(TestRequests.NewCategory("A-only"))).Content;

        var userB = await CreateAuthenticatedApiAsync(); // different real user (own auth cookie)
        var response = await userB.Categories.Get(id);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // RLS hides A's row from B on reads too
    }
}
