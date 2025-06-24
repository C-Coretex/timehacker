namespace TimeHacker.Domain.Contracts.BusinessLogicExceptions
{
    public class DataIsNotCorrectException(string? message, string paramName) : ArgumentException(message, paramName)
    {
    }
}
