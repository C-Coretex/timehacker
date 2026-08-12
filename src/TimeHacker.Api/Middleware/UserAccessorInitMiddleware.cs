using System.Diagnostics;
using TimeHacker.Domain.Observability;

namespace TimeHacker.Api.Middleware;

/// <summary>
/// Runs <see cref="UserAccessor.Init"/> once per request (after auth) so the domain user context is
/// resolved and cached before any controller executes. All the logic lives in <see cref="UserAccessor"/>.
/// Once the user is known, this is also the one place that can attribute the whole request to a tenant,
/// so it stamps the id onto the trace, the logs, and the active-user tally.
/// </summary>
internal sealed class UserAccessorInitMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    public async Task InvokeAsync(HttpContext context, UserAccessor accessor, ILogger<UserAccessorInitMiddleware> logger)
    {
        await accessor.Init();

        if (accessor.UserId is not { } userId)
        {
            await _next(context);
            return;
        }

        // Tag the ambient request (server) span with the resolved domain user so every trace is
        // attributable to a tenant — every query is user-scoped.
        Activity.Current?.SetTag(TimeHackerTelemetry.EndUserIdTagName, userId.ToString());

        ActiveUserTracker.Touch(userId);

        // The same id on every log record of the request. Through OTLP this becomes Loki structured
        // metadata (`enduser_id`), which is what makes exact distinct-user and per-user activity queries
        // possible without ever putting a user id on a metric.
        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            [TimeHackerTelemetry.EndUserIdTagName] = userId
        });

        await _next(context);
    }
}
