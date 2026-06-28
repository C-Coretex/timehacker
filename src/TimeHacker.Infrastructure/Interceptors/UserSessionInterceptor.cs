using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace TimeHacker.Infrastructure.Interceptors;

public class UserSessionInterceptor(UserAccessorBase userAccessor) : DbConnectionInterceptor
{
    public const string SessionUserIdParameterName = "app.user_id";

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        
        // Set the user_id parameter for the current session
        var userId = userAccessor.GetUserIdOrThrowUnauthorized();

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
