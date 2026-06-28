using AutoBogus;
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
