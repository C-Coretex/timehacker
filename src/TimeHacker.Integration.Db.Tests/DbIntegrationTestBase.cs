using AutoBogus;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Infrastructure;
using TimeHacker.Integration.Db.Tests.Fixtures;
using TimeHacker.Tests.Helpers.AutoFaker;

namespace Timehacker.Integration.Db.Tests;

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

    protected DbIntegrationTestBase(DbContainerFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        _fixture = fixture;
        CurrentUser = new UserFixture(fixture.ConnectionString);
        OtherUsers = [.. Enumerable.Range(0, 3).Select(_ => new UserFixture(fixture.ConnectionString))];

        _dbContext = new TimeHackerDbContext(fixture.AdminConnectionString);
    }

    protected T Resolve<T>() where T : notnull
        => CurrentUser.Resolve<T>();

    protected TimeHackerDbContext Db
        => _dbContext;

    /// <summary>
    /// Builds a raw <see cref="TimeHackerDbContext"/> on the RLS-bound <c>application_user</c> connection
    /// with <c>app.user_id</c> set to <paramref name="userId"/> — no repository, no interceptor, no in-app
    /// guard. Used to verify PostgreSQL RLS in isolation by calling <c>Set&lt;T&gt;()</c> directly.
    /// </summary>
    protected async Task<TimeHackerDbContext> CreateRlsContextAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var context = new TimeHackerDbContext(_fixture.ConnectionString);
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
        await Task.WhenAll([CurrentUser.DisposeAsync().AsTask(), .. OtherUsers.Select(u => u.DisposeAsync().AsTask())]);
        await _fixture.ResetAsync();
        await _dbContext.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
