using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TimeHacker.Infrastructure.Identity;

public class TimeHackerIdentityDbContext : IdentityDbContext<IdentityUser>
{
    public TimeHackerIdentityDbContext(DbContextOptions<TimeHackerIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
