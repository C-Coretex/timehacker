using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TimeHacker.Infrastructure.Identity
{
    public class TimeHackerIdentityDbContext : IdentityDbContext<IdentityUser>
    {
        public TimeHackerIdentityDbContext(DbContextOptions<TimeHackerIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applies all configurations defined in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
