namespace TimeHacker.Api.Middleware;

public class UserAccessorInitMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    public async Task InvokeAsync(HttpContext context, UserAccessor accessor)
    {
        await accessor.Init();
        await _next(context);
    }
}
