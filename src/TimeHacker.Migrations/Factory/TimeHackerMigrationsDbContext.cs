using TimeHacker.Infrastructure;
using TimeHacker.Migrations.Configuration;

namespace TimeHacker.Migrations.Factory;

public class TimeHackerMigrationsDbContext : DbContext
{
    public TimeHackerMigrationsDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);
        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimeHackerDbContext).Assembly);
    }

    public static void ApplyMigrations(string connectionString)
    {
        // Swap in the RLS-aware differ (same as the design-time factory) so applying migrations at startup
        // produces the same RLS policy DDL that `dotnet ef` would scaffold. Connect as the table owner
        // (postgres) — enabling RLS / creating policies requires owner privileges.
        var optionsBuilder = new DbContextOptionsBuilder().UseNpgsql(connectionString)
            .ReplaceService<IMigrationsModelDiffer, RlsMigrationsModelDiffer>();
        using var context = new TimeHackerMigrationsDbContext(optionsBuilder.Options);
        var db = context.Database;
        var pendingMigrations = db.GetPendingMigrations();

        if (pendingMigrations.Any())
            db.Migrate();
    }
}
