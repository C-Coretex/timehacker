namespace TimeHacker.Domain.Contracts.BusinessLogicExceptions
{
    public class NotProvidedException(string paramName) : ArgumentException("", paramName)
    {
    }
}
