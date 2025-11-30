namespace TimeHacker.Domain.BusinessLogicExceptions;

public class NotProvidedException(string paramName) : ArgumentException("", paramName)
{
}
