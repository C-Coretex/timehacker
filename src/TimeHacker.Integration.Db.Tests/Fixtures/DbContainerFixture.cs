using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using TimeHacker.Migrations.Factory;

namespace TimeHacker.Integration.Db.Tests.Fixtures;

public class DbContainerFixture: IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("timehacker_test")
        .Build();

    public string ConnectionString => _container.GetConnectionString();
    public Respawner Respawner { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        TimeHackerMigrationsDbContext.ApplyMigrations(ConnectionString);

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        Respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"],
        });
    }

    public async ValueTask ResetAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await Respawner.ResetAsync(connection);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
