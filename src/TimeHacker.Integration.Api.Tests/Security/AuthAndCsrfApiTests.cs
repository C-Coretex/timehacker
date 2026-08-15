namespace TimeHacker.Integration.Api.Tests.Security;

public sealed class AuthAndCsrfApiTests(ApiTestFixture fixture) : ApiIntegrationTestBase(fixture)
{
    [Fact, Trait("Security", "Auth")]
    public async Task Endpoints_Should_Return401_WhenUnauthenticated()
    {
        var api = CreateAnonymousApi();

        (await api.Categories.GetAll()).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await api.FixedTasks.GetAll()).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await api.DynamicTasks.GetAll()).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await api.Users.GetCurrent()).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await api.Tasks.GetForDay("2026-07-01")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await api.Categories.Create(TestRequests.NewCategory())).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact, Trait("Security", "CSRF")]
    public async Task MutatingRequest_WithoutAntiforgeryToken_Should_BeRejected()
    {
        // Authenticated, but the antiforgery token was never loaded onto the client.
        var api = await CreateAuthenticatedApiAsync(loadCsrf: false);

        var create = await api.Categories.Create(TestRequests.NewCategory());

        create.StatusCode.Should().Be(HttpStatusCode.BadRequest); // AntiforgeryValidationFailedResult
    }

    [Fact, Trait("Security", "CSRF")]
    public async Task SafeGet_WithoutAntiforgeryToken_Should_Succeed()
    {
        var api = await CreateAuthenticatedApiAsync(loadCsrf: false);

        var response = await api.Categories.GetAll();

        response.StatusCode.Should().Be(HttpStatusCode.OK); // GET is exempt from antiforgery
    }

    [Fact, Trait("Security", "CSRF token endpoint")]
    public async Task AntiforgeryToken_Should_RequireAuth_AndReturnToken()
    {
        (await CreateAnonymousApi().Auth.GetAntiforgeryToken()).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var authed = await CreateAuthenticatedApiAsync(loadCsrf: false);
        var token = await authed.Auth.GetAntiforgeryToken();
        token.StatusCode.Should().Be(HttpStatusCode.OK);
        token.Content!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact, Trait("Security", "RLS isolation")]
    public async Task Mutations_ByAnotherUser_Should_Return404_AndNotAffectFixedTask()
    {
        var userA = await CreateAuthenticatedApiAsync();
        var taskId = (await userA.FixedTasks.Create(TestRequests.NewFixedTask("A-owned"))).Content;

        var userB = await CreateAuthenticatedApiAsync();
        // B sees 0 rows under RLS, so both update and delete report 404 and A's row is untouched.
        (await userB.FixedTasks.Update(taskId, TestRequests.NewFixedTask("hacked"))).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await userB.FixedTasks.Delete(taskId)).StatusCode.Should().Be(HttpStatusCode.NotFound);

        var stillThere = await userA.FixedTasks.Get(taskId);
        stillThere.StatusCode.Should().Be(HttpStatusCode.OK);
        stillThere.Content!.Name.Should().Be("A-owned");
    }
}
