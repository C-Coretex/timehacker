namespace TimeHacker.Domain.BusinessLogicExceptions;

#pragma warning disable CA1032
public class NotProvidedException : ArgumentException
#pragma warning restore CA1032
{
    public NotProvidedException(string paramName)
        : base("", paramName) { }

    public NotProvidedException(string propertyName, string paramName)
        : base($"The {propertyName} property of {paramName} was not provided.", propertyName) { }
}
