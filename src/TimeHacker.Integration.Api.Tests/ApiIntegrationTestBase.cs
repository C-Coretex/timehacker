using Microsoft.AspNetCore.Identity.Data;
using TimeHacker.Infrastructure;

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

    // Anonymous composite client (no auth cookie) — for 401 checks.
    protected TimeHackerApi CreateAnonymousApi() => new(fixture.CreateApiClient());

    // Register + login a brand-new user; by default also load the CSRF token so mutating verbs pass
    // antiforgery. Pass loadCsrf: false to exercise the antiforgery-rejection path.
    protected async Task<TimeHackerApi> CreateAuthenticatedApiAsync(bool loadCsrf = true)
    {
        var httpClient = fixture.CreateApiClient();
        var api = new TimeHackerApi(httpClient);
        var (email, password) = TestUser.New();

        await api.Auth.Register(new RegisterRequest { Email = email, Password = password });
        await api.Auth.Login(new LoginRequest { Email = email, Password = password });

        if (loadCsrf)
        {
            var token = await api.Auth.GetAntiforgeryToken();
            httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token.Content!.Token);
        }

        return api;
    }

    public virtual async ValueTask DisposeAsync()
    {
        await AdminDbContext.DisposeAsync();
        await Fixture.ResetAsync();

        GC.SuppressFinalize(this);
    }
}
