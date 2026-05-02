using TimeHacker.Infrastructure;

namespace TimeHacker.Migrations.Factory;

public class TimeHackerMigrationsDbContext : DbContext
{
    public TimeHackerMigrationsDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder, nameof(modelBuilder));
        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimeHackerDbContext).Assembly);
    }

    public static void ApplyMigrations(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder().UseNpgsql(connectionString);
        using var context = new TimeHackerMigrationsDbContext(optionsBuilder.Options);
        var db = context.Database;
        var pendingMigrations = db.GetPendingMigrations();

        if (pendingMigrations.Any())
            db.Migrate();
    }
}
