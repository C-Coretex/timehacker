using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TimeHacker.Infrastructure.Identity;

namespace TimeHacker.Migrations.Identity.Factory;

public class IdentityMigrationsDbContext : IdentityDbContext<IdentityUser>
{
    public IdentityMigrationsDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        // Apply all configurations
        builder.ApplyConfigurationsFromAssembly(typeof(TimeHackerIdentityDbContext).Assembly);
    }

    public static void ApplyMigrations(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder().UseNpgsql(connectionString);

        using var context = new IdentityMigrationsDbContext(optionsBuilder.Options);
        var db = context.Database;    
        var pendingMigrations = db.GetPendingMigrations();

        if (pendingMigrations.Any())
            db.Migrate();
    }
}
