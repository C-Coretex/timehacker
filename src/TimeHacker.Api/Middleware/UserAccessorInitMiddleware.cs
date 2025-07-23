using TimeHacker.Api.Helpers;

namespace TimeHacker.Api.Middleware
{
    public class UserAccessorInitMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, UserAccessor accessor)
        {
            await accessor.Init();
            await next(context);
        }
    }
}
