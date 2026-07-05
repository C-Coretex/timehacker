namespace TimeHacker.Infrastructure.Factories;

// Bridges DbContext pooling with per-request RLS state. Leases a pooled TimeHackerDbContext from the
// singleton pooled factory and stamps the current scope's IServiceProvider onto it, so
// UserSessionInterceptor can resolve the scoped UserAccessorBase lazily at connection-open.
//
// We stash the provider (not UserAccessorBase itself) on purpose: UserAccessorBase -> UserAccessor ->
// IUserRepository -> TimeHackerDbContext, so resolving the accessor while creating the context would form
// a DI cycle. Because this factory is registered Scoped, the injected IServiceProvider is the scope's provider.
public sealed class TimeHackerScopedDbContextFactory(
    IDbContextFactory<TimeHackerDbContext> pooledFactory,
    IServiceProvider scopeProvider) : IDbContextFactory<TimeHackerDbContext>
{
    public TimeHackerDbContext CreateDbContext()
    {
        var context = pooledFactory.CreateDbContext();
        context.ScopeServiceProvider = scopeProvider;
        return context;
    }
}
