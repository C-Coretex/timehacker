using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using TimeHacker.Infrastructure;
using TimeHacker.Migrations.Factory;
using TimeHacker.Migrations.Identity.Factory;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

public sealed class ApiTestFixture : IAsyncLifetime
{
    private const string AppUser = "application_user";
    private const string AppUserPassword = "application_password";

    private readonly PostgreSqlContainer _mainDb = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("TimeHacker")
        .WithResourceMapping(
            new FileInfo("./Resources/timehacker_infrastructure_init.sql"),
            new FileInfo("/docker-entrypoint-initdb.d/timehacker_infrastructure_init.sql"))
        .Build();

    private readonly PostgreSqlContainer _identityDb =
        new PostgreSqlBuilder("postgres:latest").WithDatabase("TimeHackerIdentity").Build();

    private readonly string _keysPath = Path.Combine(Path.GetTempPath(), "th-keys-" + Guid.NewGuid());
    private TimeHackerApiFactory _factory = null!;
    private readonly List<TimeHackerApiFactory> _brokenFactories = [];
    private Respawner _mainRespawner = null!, _identityRespawner = null!;

    public string MainAdminConnectionString => _mainDb.GetConnectionString();
    public string MainAppConnectionString => new NpgsqlConnectionStringBuilder(_mainDb.GetConnectionString())
    { 
        Username = AppUser, 
        Password = AppUserPassword 
    }.ConnectionString;
    public string IdentityConnectionString => _identityDb.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(_keysPath);
        await Task.WhenAll(_mainDb.StartAsync(), _identityDb.StartAsync());

        TimeHackerMigrationsDbContext.ApplyMigrations(MainAdminConnectionString);
        IdentityMigrationsDbContext.ApplyMigrations(IdentityConnectionString);

        _mainRespawner = await CreateRespawner(MainAdminConnectionString);
        _identityRespawner = await CreateRespawner(IdentityConnectionString);

        _factory = new TimeHackerApiFactory(MainAppConnectionString, MainAdminConnectionString, IdentityConnectionString, _keysPath);

        // Force the shared host to build now (reading the good connection-string env vars) so its config
        // is locked in. CreateBrokenDbApiClient later mutates those process-global env vars to build a
        // separate broken host; warming up here keeps the shared factory unaffected.
        using var warmup = _factory.CreateClient();
    }

    // Client that stores/sends Secure+SameSite=None cookies and avoids the https-redirect no-op.
    public HttpClient CreateApiClient() => _factory.CreateClient(new WebApplicationFactoryClientOptions
    { 
        BaseAddress = new Uri("https://localhost")
    });

    // Admin context for assertions — bypasses RLS, sees every user's rows.
    public TimeHackerDbContext CreateAdminDbContext() => new(MainAdminConnectionString);

    // A client whose API host points its main DB at an unreachable endpoint, so the "TimeHackerDb"
    // health check reports Unhealthy. Used to assert GET /health -> 503. The shared factory was warmed
    // in InitializeAsync, so mutating the connection-string env vars here doesn't affect it.
    public HttpClient CreateBrokenDbApiClient()
    {
        const string unreachableMainDb =
            "Host=localhost;Port=1;Database=nonexistent;Username=none;Password=none;Timeout=1;Command Timeout=1";

        var brokenFactory = new TimeHackerApiFactory(unreachableMainDb, MainAdminConnectionString, IdentityConnectionString, _keysPath);
        _brokenFactories.Add(brokenFactory);

        var client = brokenFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = true
        });

        // Restore the good env for anything built afterwards (defensive — the shared factory is already built).
        Environment.SetEnvironmentVariable("ConnectionStrings__TimeHackerConnectionString", MainAppConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__TimeHackerAdminConnectionString", MainAdminConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__IdentityConnectionString", IdentityConnectionString);

        return client;
    }

    public async ValueTask ResetAsync()
    {
        await Reset(_mainRespawner, MainAdminConnectionString);
        await Reset(_identityRespawner, IdentityConnectionString);
    }

    private static async Task Reset(Respawner respawner, string connectionString)
    { 
        await using var connection = new NpgsqlConnection(connectionString); 
        await connection.OpenAsync(); 
        await respawner.ResetAsync(connection); 
    }

    private static async Task<Respawner> CreateRespawner(string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString); 
        await connection.OpenAsync();

        return await Respawner.CreateAsync(connection, new RespawnerOptions
        { 
            DbAdapter = DbAdapter.Postgres, 
            TablesToIgnore = ["__EFMigrationsHistory"] 
        });
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var brokenFactory in _brokenFactories)
            await brokenFactory.DisposeAsync();

        await _factory.DisposeAsync();
        await _mainDb.DisposeAsync();
        await _identityDb.DisposeAsync();

        Directory.Delete(_keysPath, true);
    }
}
