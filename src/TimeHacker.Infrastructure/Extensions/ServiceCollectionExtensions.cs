using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.Tags;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Infrastructure.Factories;
using TimeHacker.Infrastructure.Interceptors;
using TimeHacker.Infrastructure.Repositories.Categories;
using TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;
using TimeHacker.Infrastructure.Repositories.Tags;
using TimeHacker.Infrastructure.Repositories.Tasks;
using TimeHacker.Infrastructure.Repositories.Users;

namespace TimeHacker.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services, string timeHackerConnectionString)
    {
        // Interceptors must be Singletons: DbContext pooling builds the options (and bakes in the
        // interceptor instances) once from the root provider, so scoped/transient interceptors would throw
        // "Cannot resolve scoped service from root provider". Both are stateless — the per-request UserId is
        // carried on the leased context by TimeHackerScopedDbContextFactory (see UserSessionInterceptor).
        services.AddSingleton<UserSessionInterceptor>();
        services.AddSingleton<TimestampInterceptor>();

        // Build a named NpgsqlDataSource so Npgsql emits OpenTelemetry metrics (connection pool, command
        // duration, etc.) tagged with a stable pool name. A NpgsqlDataSource owns a connection pool, so it
        // MUST be disposed or its connections leak. It is registered as a *keyed* singleton built by a
        // factory (not a pre-built instance): the DI container only disposes IDisposables it creates, so a
        // bare `AddSingleton(instance)` would never be disposed — each rebuilt provider (e.g. per integration
        // test) would leak a full pool until PostgreSQL runs out of connection slots. Resolving it from `sp`
        // below forces the singleton to be created and thus tracked for disposal; the key keeps this pool
        // distinct from the identity DB's data source when both live in the same container.
        // Interceptors/RLS are unaffected — UserSessionInterceptor is a DbConnectionInterceptor that runs
        // regardless of how the connection is sourced.
        services.AddKeyedSingleton("TimeHacker", (_, _) =>
            new NpgsqlDataSourceBuilder(timeHackerConnectionString) { Name = "TimeHacker" }.Build());

        services.AddPooledDbContextFactory<TimeHackerDbContext>((sp, options) =>
        {
            options.UseNpgsql(sp.GetRequiredKeyedService<NpgsqlDataSource>("TimeHacker"));
            options.AddInterceptors(
                sp.GetRequiredService<UserSessionInterceptor>(),
                sp.GetRequiredService<TimestampInterceptor>());
        });

        // Hand repositories a scoped, pooled context whose ScopeServiceProvider points at the current scope.
        services.AddScoped<TimeHackerScopedDbContextFactory>();
        services.AddScoped(sp => sp.GetRequiredService<TimeHackerScopedDbContextFactory>().CreateDbContext());

        // Same DbContext is shared between repositories in the same scope, so transactions would work out of the box
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<IScheduleSnapshotRepository, ScheduleSnapshotRepository>();
        services.AddScoped<IScheduledTaskRepository, ScheduledTaskRepository>();
        services.AddScoped<IScheduledCategoryRepository, ScheduledCategoryRepository>();
        services.AddScoped<IScheduleEntityRepository, ScheduleEntityRepository>();

        services.AddScoped<IFixedTaskRepository, FixedTaskRepository>();
        services.AddScoped<IDynamicTaskRepository, DynamicTaskRepository>();

        services.AddScoped<ITagRepository, TagRepository>();

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
