using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace TimeHacker.Infrastructure.Interceptors;

// Resolves UserAccessorBase lazily (at connection-open time) rather than via the constructor.
// The interceptor is created while TimeHackerDbContext is being built, and UserAccessorBase ->
// UserAccessor -> IUserRepository -> TimeHackerDbContext, so taking UserAccessorBase as a ctor
// dependency forms a DI resolution cycle that deadlocks/recurses during DbContext construction.
// By the time a connection opens, the DbContext already exists in the scope, so resolving the
// accessor here returns the cached instance without re-entering DbContext construction.
public class UserSessionInterceptor(IServiceProvider serviceProvider) : DbConnectionInterceptor
{
    public const string SessionUserIdParameterName = "app.user_id";

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);

        // Set the user_id parameter for the current session
        var userAccessor = serviceProvider.GetRequiredService<UserAccessorBase>();
        var userId = userAccessor.UserId ?? Guid.NewGuid(); //new guid to filter out all users if userId not found

#pragma warning disable CA1062 // Validate arguments of public methods
        using var command = connection.CreateCommand();
#pragma warning restore CA1062 // Validate arguments of public methods
        command.CommandText = $"SELECT set_config('{SessionUserIdParameterName}', @userId, false);";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@userId";
        parameter.Value = userId.ToString();
        command.Parameters.Add(parameter);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
