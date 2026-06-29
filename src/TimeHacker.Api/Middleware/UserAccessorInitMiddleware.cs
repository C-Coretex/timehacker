namespace TimeHacker.Api.Middleware;

/// <summary>
/// Runs <see cref="UserAccessor.Init"/> once per request (after auth) so the domain user context is
/// resolved and cached before any controller executes. All the logic lives in <see cref="UserAccessor"/>.
/// </summary>
internal sealed class UserAccessorInitMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    public async Task InvokeAsync(HttpContext context, UserAccessor accessor)
    {
        await accessor.Init();
        await _next(context);
    }
}
