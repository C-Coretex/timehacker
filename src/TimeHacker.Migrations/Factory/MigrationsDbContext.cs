using Microsoft.EntityFrameworkCore;
using TimeHacker.Infrastructure;

namespace TimeHacker.Migrations.Factory
{
    public class MigrationsDbContext : DbContext
    {
        public MigrationsDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimeHackerDbContext).Assembly);
        }
    }
}
