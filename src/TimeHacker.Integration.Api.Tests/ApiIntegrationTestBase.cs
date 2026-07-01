using TimeHacker.Infrastructure;
using TimeHacker.Integration.Api.Tests.Fixtures;

namespace TimeHacker.Integration.Api.Tests;

public abstract class ApiIntegrationTestBase(ApiTestFixture fixture) : IAsyncLifetime
{
    protected ApiTestFixture Fixture => fixture;
    protected TimeHackerDbContext AdminDbContext { get; private set; } = null!; // admin — for side-effect assertions

    public virtual ValueTask InitializeAsync()
    {
        AdminDbContext = fixture.CreateAdminDbContext();
        return ValueTask.CompletedTask; 
    }

    protected HttpClient CreateAnonymousClient() => Fixture.CreateApiClient();

    // TestServer has no port; requests use relative URIs against the client BaseAddress.
    internal static Uri Url(string relativeUrl) => ApiClientExtensions.GetUri(relativeUrl);

    // Register + login a brand-new user, then load the CSRF token so mutating verbs pass antiforgery.
    protected async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = fixture.CreateApiClient();
        var (email, password) = TestUser.New();

        (await client.RegisterAsync(email, password)).EnsureSuccessStatusCode();
        (await client.LoginAsync(email, password)).EnsureSuccessStatusCode();
        await client.LoadCsrfTokenAsync();

        return client;
    }

    public virtual async ValueTask DisposeAsync()
    { 
        await AdminDbContext.DisposeAsync(); 
        await Fixture.ResetAsync();

        GC.SuppressFinalize(this);
    }
}
