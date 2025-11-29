using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TimeHacker.Infrastructure.Identity;

namespace TimeHacker.Migrations.Identity.Factory
{
    public class IdentityMigrationsDbContext : IdentityDbContext<IdentityUser>
    {
        public IdentityMigrationsDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimeHackerIdentityDbContext).Assembly);
        }

        public static void ApplyMigrations(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseNpgsql(connectionString);
            var db = new IdentityMigrationsDbContext(optionsBuilder.Options).Database;
            var pendingMigrations = db.GetPendingMigrations();

            if (pendingMigrations.Any())
                db.Migrate();
        }
    }
}
