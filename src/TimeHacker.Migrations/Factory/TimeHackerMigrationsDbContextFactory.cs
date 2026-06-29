using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TimeHacker.Migrations.Configuration;

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

        // Design-time (`dotnet ef migrations add`) must use the RLS-aware differ too, otherwise scaffolded
        // migrations would omit the ENABLE RLS / CREATE POLICY statements. Mirrors ApplyMigrations.
        var optionsBuilder = new DbContextOptionsBuilder().UseNpgsql(connectionString)
            .ReplaceService<IMigrationsModelDiffer, RlsMigrationsModelDiffer>();
        return new TimeHackerMigrationsDbContext(optionsBuilder.Options);
    }
}
