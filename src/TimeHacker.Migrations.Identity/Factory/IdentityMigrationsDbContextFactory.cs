using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TimeHacker.Migrations.Identity.Factory;

public class IdentityMigrationsDbContextFactory : IDesignTimeDbContextFactory<IdentityMigrationsDbContext>
{
    public IdentityMigrationsDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString(nameof(IdentityMigrationsDbContext));
        var optionsBuilder = new DbContextOptionsBuilder().UseNpgsql(connectionString);

        return new IdentityMigrationsDbContext(optionsBuilder.Options);
    }
}
