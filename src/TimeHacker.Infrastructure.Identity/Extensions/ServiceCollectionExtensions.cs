using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TimeHacker.Infrastructure.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterIdentity(this IServiceCollection services, string identityConnectionString)
    {
        // Named NpgsqlDataSource so the identity DB's Npgsql pool/command metrics are tagged with a stable
        // pool name (see the main infrastructure registration for details). Registered as a *keyed* singleton
        // built by a factory and resolved from `sp` so the container owns and disposes its connection pool —
        // a bare `AddSingleton(instance)` is never disposed by the container and leaks connections per rebuilt
        // provider. The key keeps it distinct from the main DB's data source in the same container.
        services.AddKeyedSingleton("TimeHackerIdentity", (_, _) =>
            new NpgsqlDataSourceBuilder(identityConnectionString) { Name = "TimeHackerIdentity" }.Build());

        services.AddDbContext<TimeHackerIdentityDbContext>((sp, options) =>
            options.UseNpgsql(sp.GetRequiredKeyedService<NpgsqlDataSource>("TimeHackerIdentity")));

        return services;
    }
}
