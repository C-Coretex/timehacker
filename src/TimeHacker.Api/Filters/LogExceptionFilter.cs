using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace TimeHacker.Api.Filters;

public class LogExceptionFilter(ILoggerFactory loggerFactory, IWebHostEnvironment environment) : IExceptionFilter
{
    private const string ExceptionTypeExtensionName = "ExceptionType";
    private const string ParameterNameExtensionName = "ParameterName";

    public void OnException(ExceptionContext context)
    {
        var exceptionProcessed = ProcessException(context, out var objectResult);
        if (!exceptionProcessed) 
            return;

        context.Result = objectResult;
        context.ExceptionHandled = true;
    }

    /// <returns>true - if exception should be handled with output parameter</returns>
    private bool ProcessException(ExceptionContext context, out ObjectResult objectResult)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = environment.IsDevelopment() ? $"{context.Exception.Message}\n{context.Exception.StackTrace}" : null
        };
        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        problemDetails.Extensions["traceId"] = traceId;
        objectResult = new ObjectResult(problemDetails)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        //we don't want to log internal business logic exceptions
        switch (context.Exception)
        {
            case UserAlreadyPresentException:
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "User data is already present.";
                problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(UserAlreadyPresentException);
                objectResult = new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status409Conflict };

                return true;
            case UserDoesNotExistException:
                problemDetails.Title = "User data does not exist.";
                problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(UserDoesNotExistException);

                return true;
            case NotFoundException exception:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Resource is not found.";
                problemDetails.Detail = $"Resource '{exception.ResourceName}' by Id '{exception.ResourceId}' is not found.";
                problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(NotFoundException);
                problemDetails.Extensions["ResourceName"] = exception.ResourceName;
                problemDetails.Extensions["Identifier"] = exception.ResourceId;
                objectResult = new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status404NotFound };

                return true;
            case NotProvidedException exception:
                problemDetails.Title = "Parameter is not provided.";
                problemDetails.Detail = $"Parameter '{exception.ParamName}' is not provided.";
                problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(NotProvidedException);
                problemDetails.Extensions[ParameterNameExtensionName] = exception.ParamName;

                return true;
            case DataIsNotCorrectException exception:
                problemDetails.Title = "Parameter is not correct.";
                problemDetails.Detail = $"Parameter '{exception.ParamName}' is not correct.";
                problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(DataIsNotCorrectException);
                problemDetails.Extensions[ParameterNameExtensionName] = exception.ParamName;

                return true;
            default:
                LogException(context);
                return false;
        }
    }

    private void LogException(ExceptionContext context)
    {
        var controllerName = context.ActionDescriptor.RouteValues["controller"];
        var actionName = context.ActionDescriptor.RouteValues["action"];

        var logger = loggerFactory.CreateLogger($"Controller:{controllerName} - Action:{actionName}");

        logger.LogError(context.Exception,
            "Unhandled exception in controller '{Controller}', action '{Action}', path: {Path}",
            controllerName,
            actionName,
            SanitizeValue(context.HttpContext.Request.Path.Value));
    }

    private static string SanitizeValue(string? value)
    {
        return value?.Replace("\r", string.Empty).Replace("\n", string.Empty) ?? string.Empty;
    }
}
