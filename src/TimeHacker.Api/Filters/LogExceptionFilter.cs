using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TimeHacker.Domain.BusinessLogicExceptions;

namespace TimeHacker.Api.Filters
{
    public class LogExceptionFilter(ILoggerFactory loggerFactory) : IExceptionFilter
    {
        private const string ExceptionTypeExtensionName = "ExceptionType";

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
                Detail = context.Exception.Message
            };
            objectResult = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            //we don't want to log internal business logic exceptions
            switch (context.Exception)
            {
                case UserAlreadyPresentException:
                    problemDetails.Title = "User data is already present.";
                    problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(UserAlreadyPresentException);

                    return true;
                case UserDoesNotExistException:
                    problemDetails.Title = "User data does not exist.";
                    problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(UserDoesNotExistException);

                    return true;
                case NotFoundException exception:
                    problemDetails.Title = $"Resource '{exception.ResourceName}' by Id '{exception.ResourceId}' is not found.";
                    problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(NotFoundException);
                    problemDetails.Extensions["ResourceName"] = exception.ResourceName;
                    problemDetails.Extensions["Identifier"] = exception.ResourceId;

                    return true;
                case NotProvidedException exception:
                    problemDetails.Title = $"Parameter '{exception.ParamName}' is not provided.";
                    problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(NotProvidedException);
                    problemDetails.Extensions["ParameterName"] = exception.ParamName;

                    return true;
                case DataIsNotCorrectException exception:
                    problemDetails.Title = $"Parameter '{exception.ParamName}' is not correct.";
                    problemDetails.Extensions[ExceptionTypeExtensionName] = nameof(DataIsNotCorrectException);
                    problemDetails.Extensions["ParameterName"] = exception.ParamName;

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
                context.HttpContext.Request.Path);
        }
    }
}
