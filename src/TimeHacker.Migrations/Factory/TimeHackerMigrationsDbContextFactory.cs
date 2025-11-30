using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TimeHacker.Migrations.Factory;

public class MigrationsDbContextFactory : IDesignTimeDbContextFactory<TimeHackerMigrationsDbContext>
{
    public TimeHackerMigrationsDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString(nameof(TimeHackerMigrationsDbContext));

        var optionsBuilder = new DbContextOptionsBuilder().UseNpgsql(connectionString);
        return new TimeHackerMigrationsDbContext(optionsBuilder.Options);
    }
}
