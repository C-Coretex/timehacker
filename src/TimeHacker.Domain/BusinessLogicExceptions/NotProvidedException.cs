using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TimeHacker.Domain.BusinessLogicExceptions;

#pragma warning disable CA1032
public class NotProvidedException : ArgumentException
#pragma warning restore CA1032
{
    public NotProvidedException(string paramName)
        : base("", paramName) { }

    public NotProvidedException(string propertyName, string paramName)
        : base($"The {propertyName} property of {paramName} was not provided.", propertyName) { }

    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string paramName = "", string? propertyName = null)
    {
        if (argument is null)
        {
            if(propertyName is not null)
                throw new NotProvidedException(propertyName, paramName);

            throw new NotProvidedException(paramName);
        }
    }
}
