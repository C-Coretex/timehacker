#pragma warning disable CA1062 // Validate arguments of public methods

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace TimeHacker.Infrastructure.Interceptors;

// Sets the PostgreSQL session variable `app.user_id` on every connection open, which the RLS policies use
// to scope rows to the current user.
//
// Registered as a stateless Singleton so it is compatible with DbContext pooling (pooling bakes interceptor
// instances into the singleton options once, so a scoped/transient interceptor can't be used here).
// The per-request UserId is resolved lazily from the leased context's scope: TimeHackerScopedDbContextFactory
// stamps the current scope's IServiceProvider onto the context, and we resolve UserAccessorBase from it at
// connection-open. Resolving lazily (rather than taking UserAccessorBase as a dependency) avoids a DI cycle
// — UserAccessorBase -> UserAccessor -> IUserRepository -> TimeHackerDbContext — because by connection-open
// the context graph is already built and cached in the scope.
// We can't inject IServiceProvider directly because the UserSessionInterceptor is Singleton and would not be resolved for each scope.
public class UserSessionInterceptor : DbConnectionInterceptor
{
    public const string SessionUserIdParameterName = "app.user_id";

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);

        var userAccessor = (eventData.Context as TimeHackerDbContext)?.ScopeServiceProvider?.GetService<UserAccessorBase>();
        var userId = userAccessor?.UserId ?? Guid.NewGuid(); //new guid to filter out all users if userId not found (we can't just throw as the table requested can be without RLS)

        // Set the user_id parameter for the current session
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT set_config('{SessionUserIdParameterName}', @userId, false);";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@userId";
        parameter.Value = userId.ToString();
        command.Parameters.Add(parameter);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
