using AutoBogus;
using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Tests.Helpers.AutoFaker;

namespace TimeHacker.Integration.Db.Tests;

public abstract class DbIntegrationTestBase: IAsyncLifetime
{
    static DbIntegrationTestBase()
    {
        AutoFaker.Configure(builder =>
        {
            builder.WithTreeDepth(1); builder.WithRepeatCount(0); 
            builder.WithBinder(new AggregateBinder());
        });
    }

    protected UserFixture CurrentUser { get; }
    protected IReadOnlyCollection<UserFixture> OtherUsers { get; }

    private readonly TimeHackerDbContext _dbContext;
    private readonly DbContainerFixture _fixture;
    private readonly List<IServiceScope> _extraScopes = [];

    protected DbIntegrationTestBase(DbContainerFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        _fixture = fixture;
        CurrentUser = new UserFixture(fixture.ConnectionString);
        OtherUsers = [.. Enumerable.Range(0, 3).Select(_ => new UserFixture(fixture.ConnectionString))];

        _dbContext = TimeHackerDbContext.Create(fixture.AdminConnectionString);
    }

    protected T Resolve<T>() where T : notnull
        => CurrentUser.Resolve<T>();

    /// <summary>
    /// Resolves from the scope whose DbContext is shared across repositories, as production shares it.
    /// Needed by a test that attaches to <see cref="TimeHackerDbContext"/> and then saves through a
    /// repository, or that otherwise depends on two resolutions seeing one change tracker — the acting
    /// scope hands out a fresh context per resolution and those two would never meet.
    /// </summary>
    protected T ResolveShared<T>() where T : notnull
        => CurrentUser.ResolveShared<T>();

    /// <summary>Resolves in a brand-new acting scope — a fresh "request", disposed with the test.</summary>
    protected T ResolveInNewScope<T>() where T : notnull
    {
        var scope = CurrentUser.CreateActingScope();
        _extraScopes.Add(scope);
        return scope.ServiceProvider.GetRequiredService<T>();
    }


    protected TimeHackerDbContext Db
        => TimeHackerDbContext.Create(_fixture.AdminConnectionString);

    protected TimeHackerDbContext SharedDb
        => _dbContext;

    /// <summary>Seeds through their own DI scope, so seeded rows never enter the acting scope's change tracker.</summary>
    internal GraphSeeder Seeder
        => CurrentUser.Seeder;

    /// <summary>
    /// Builds a raw <see cref="TimeHackerDbContext"/> on the RLS-bound <c>application_user</c> connection
    /// with <c>app.user_id</c> set to <paramref name="userId"/> — no repository, no interceptor, no in-app
    /// guard. Used to verify PostgreSQL RLS in isolation by calling <c>Set&lt;T&gt;()</c> directly.
    /// </summary>
    protected async Task<TimeHackerDbContext> CreateRlsContextAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var context = TimeHackerDbContext.Create(_fixture.ConnectionString);
        // Pin one physical connection so set_config and the later Set<T>() calls share the same session.
        await context.Database.OpenConnectionAsync(cancellationToken);
        await context.Database.ExecuteSqlRawAsync("SELECT set_config('app.user_id', {0}, false)",
            [userId.ToString()], cancellationToken);
        return context;
    }

    public virtual async ValueTask InitializeAsync()
    {
        await Task.WhenAll([CurrentUser.InitializeAsync().AsTask(), ..OtherUsers.Select(u => u.InitializeAsync().AsTask())]);
    }

    public virtual async ValueTask DisposeAsync()
    {
        foreach (var scope in _extraScopes)
            scope.Dispose();

        await Task.WhenAll([CurrentUser.DisposeAsync().AsTask(), .. OtherUsers.Select(u => u.DisposeAsync().AsTask())]);
        await _fixture.ResetAsync();
        await _dbContext.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
