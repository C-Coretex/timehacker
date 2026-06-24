using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Immutable;

namespace TimeHacker.Api.Filters;

public class ValidateAntiforgeryFilter : IAsyncActionFilter
{
    private static readonly ImmutableHashSet<string> _safeMethods =
        ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, "GET", "HEAD", "OPTIONS", "TRACE");

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        if (_safeMethods.Contains(context.HttpContext.Request.Method, StringComparer.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        var antiforgery = context.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();

        try
        {
            await antiforgery.ValidateRequestAsync(context.HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            context.Result = new AntiforgeryValidationFailedResult();
            return;
        }

        await next();
    }
}
