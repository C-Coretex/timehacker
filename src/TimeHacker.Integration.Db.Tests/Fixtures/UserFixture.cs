using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Application.Api.Extensions;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Domain.Services.Extensions;
using TimeHacker.Helpers.Tests.Mocks;
using TimeHacker.Infrastructure.Extensions;
using TimeHacker.Infrastructure.Factories;

namespace TimeHacker.Integration.Db.Tests.Fixtures;

public class UserFixture: IAsyncLifetime
{
    public Guid UserId { get; } = Guid.CreateVersion7();

    private readonly ServiceProvider _actingProvider;
    private readonly ServiceProvider _seedProvider;
    private readonly IServiceScope _actingScope;
    private readonly IServiceScope _seedScope;

    public UserFixture(string connectionString)
    {
        // Two providers differing in exactly one registration: the DbContext's lifetime.
        //
        // Acting side - transient. Every Resolve hands back a context that has never seen a seeded row, so
        // a tracked read has to go to the database instead of being served from EF's identity map. That is
        // what stops a test passing on the seeder's own object graph, which is how a missing Include hides.
        //
        // Seeding side - scoped, exactly as production registers it. A seeder builds multi-entity graphs and
        // needs its repositories and its own DbContext to be one and the same: fixup across a shared change
        // tracker is what populates the navigations a seeded entity is expected to come back carrying.
        _actingProvider = BuildProvider(connectionString, transientDbContext: true);
        _seedProvider = BuildProvider(connectionString, transientDbContext: false);

        _actingScope = _actingProvider.CreateScope();
        _seedScope = _seedProvider.CreateScope();
    }

    private ServiceProvider BuildProvider(string connectionString, bool transientDbContext)
    {
        var services = new ServiceCollection();
        services.RegisterRepositories(connectionString);
        services.AddSingleton(TimeProvider.System);

        services.RegisterDomainServices();
        services.RegisterAppServices();

        services.AddScoped<UserAccessorBase>(_ => new UserAccessorBaseMock(UserId, isUserValid: true));

        services.AddScoped(typeof(SeedDataBuilder<,,>));
        services.AddScoped<GraphSeeder>();

        // Last registration wins, so this replaces the scoped one RegisterRepositories added. The factory
        // leases a fresh pooled context per call; the container still owns disposal, so each returns to the
        // pool when the scope ends.
        if (transientDbContext)
            services.AddTransient(sp => sp.GetRequiredService<TimeHackerScopedDbContextFactory>().CreateDbContext());

        return services.BuildServiceProvider();
    }

    public T Resolve<T>() where T : notnull
        => _actingScope.ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// The scope where the DbContext is shared the way production shares it. Seeders live here, and so does
    /// anything a test needs to resolve with several repositories over one change tracker - a flow whose
    /// behaviour depends on them sharing a transaction, say.
    /// </summary>
    public T ResolveShared<T>() where T : notnull
        => _seedScope.ServiceProvider.GetRequiredService<T>();

    internal GraphSeeder Seeder
        => ResolveShared<GraphSeeder>();

    /// <summary>A brand-new acting scope — a fresh "request", with its own services. Caller disposes.</summary>
    public IServiceScope CreateActingScope()
        => _actingProvider.CreateScope();

    public virtual async ValueTask InitializeAsync()
    {
        await ResolveShared<IUserRepository>().AddAndSaveAsync(new()
        {
            Id = UserId,
            Name = "Test User CURRENT",
            IdentityId = Guid.CreateVersion7().ToString()
        });
    }

    public virtual async ValueTask DisposeAsync()
    {
        _seedScope.Dispose();
        _actingScope.Dispose();
        await _seedProvider.DisposeAsync();
        await _actingProvider.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
