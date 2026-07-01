using System.Drawing;
using System.Net;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Api.Models.Input.Categories;
using TimeHacker.Api.Models.Return.Categories;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Integration.Api.Tests.Fixtures;

namespace TimeHacker.Integration.Api.Tests.TestExamples;

public sealed class EndToEndExamples(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    private static InputCategoryModel NewCategory(string name) =>
        new() { Name = name, Description = "work stuff", Color = Color.Blue };

    [Fact, Trait("Endpoint", "POST+GET /api/categories")]
    public async Task CreateAndGet_Should_RoundTripCategory()
    {
        var client = await CreateAuthenticatedClientAsync();
        var cancellationToken = TestContext.Current.CancellationToken;

        var create = await client.PostDtoAsync("/api/categories", NewCategory("Work"), cancellationToken);
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = await create.ReadJsonAsync<Guid>(cancellationToken);

        var get = await client.GetAsync(Url($"/api/categories/{id}"), cancellationToken);
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        (await get.ReadJsonAsync<CategoryReturnModel>(cancellationToken))!.Name.Should().Be("Work");

        // Admin assertion (bypasses RLS): exactly one row persisted.
        (await AdminDbContext.Set<Category>().CountAsync(cancellationToken)).Should().Be(1);
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task Get_Should_Return404_ForAnotherUsersCategory()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var userA = await CreateAuthenticatedClientAsync();
        var created = await userA.PostDtoAsync("/api/categories", NewCategory("A-only"), cancellationToken);
        var id = await created.ReadJsonAsync<Guid>(cancellationToken);

        var userB = await CreateAuthenticatedClientAsync();     // different real user (own auth cookie)
        var response = await userB.GetAsync(Url($"/api/categories/{id}"), cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // RLS hides A's row from B -> 404
    }

    [Fact, Trait("Security", "Auth")]
    public async Task Get_Should_Return401_WhenUnauthenticated()
    {
        var response = await CreateAnonymousClient().GetAsync(Url("/api/categories"), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
