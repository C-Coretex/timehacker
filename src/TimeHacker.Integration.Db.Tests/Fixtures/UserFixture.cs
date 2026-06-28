using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Application.Api.Extensions;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Domain.Services.Extensions;
using TimeHacker.Helpers.Tests.Mocks;
using TimeHacker.Infrastructure.Extensions;

namespace TimeHacker.Integration.Db.Tests.Fixtures;

public class UserFixture: IAsyncLifetime
{
    public Guid UserId { get; } = Guid.CreateVersion7();

    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;

    public UserFixture(string connectionString)
    {
        var services = new ServiceCollection();
        services.RegisterRepositories(connectionString);
        services.AddSingleton(TimeProvider.System);

        services.RegisterDomainServices();
        services.RegisterAppServices();

        services.AddScoped<UserAccessorBase>(_ => new UserAccessorBaseMock(UserId, isUserValid: true));

        services.AddScoped(typeof(SeedDataBuilder<,,>));
        services.AddScoped<GraphSeeder>();

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();
    }

    public T Resolve<T>() where T : notnull
        => _scope.ServiceProvider.GetRequiredService<T>();

    public virtual async ValueTask InitializeAsync()
    {
        var userRepository = Resolve<IUserRepository>();
        await userRepository.AddAndSaveAsync(new()
        {
            Id = UserId,
            Name = "Test User CURRENT",
            IdentityId = Guid.CreateVersion7().ToString()
        });
    }

    public virtual async ValueTask DisposeAsync()
    {
        _scope.Dispose();
        await _provider.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
