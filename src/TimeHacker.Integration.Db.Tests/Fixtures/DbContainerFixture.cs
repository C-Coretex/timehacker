using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using TimeHacker.Migrations.Factory;

namespace TimeHacker.Integration.Db.Tests.Fixtures;

public class DbContainerFixture: IAsyncLifetime
{
    private const string AppUser = "application_user";
    private const string AppUserPassword = "application_password";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:latest")
     .WithDatabase("TimeHacker")
     .WithResourceMapping(
         new FileInfo("./Resources/timehacker_infrastructure_init.sql"),
         new FileInfo("/docker-entrypoint-initdb.d/timehacker_infrastructure_init.sql"))
     .Build();

    public string AdminConnectionString => _container.GetConnectionString();

    // App connection string (application_user)
    public string ConnectionString => new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
    {
        Username = AppUser,
        Password = AppUserPassword
    }.ConnectionString;

    public Respawner Respawner { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        TimeHackerMigrationsDbContext.ApplyMigrations(AdminConnectionString);

        await using var connection = new NpgsqlConnection(AdminConnectionString);
        await connection.OpenAsync();
        Respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"],
        });
    }

    public async ValueTask ResetAsync()
    {
        await using var connection = new NpgsqlConnection(AdminConnectionString);
        await connection.OpenAsync();
        await Respawner.ResetAsync(connection);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
