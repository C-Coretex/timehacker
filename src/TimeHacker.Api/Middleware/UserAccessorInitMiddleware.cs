namespace TimeHacker.Api.Middleware;

internal sealed class UserAccessorInitMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    public async Task InvokeAsync(HttpContext context, UserAccessor accessor)
    {
        await accessor.Init().ConfigureAwait(false);
        await _next(context).ConfigureAwait(false);
    }
}
