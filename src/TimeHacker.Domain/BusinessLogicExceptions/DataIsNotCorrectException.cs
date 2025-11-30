namespace TimeHacker.Domain.BusinessLogicExceptions;

public class DataIsNotCorrectException(string? message, string paramName) : ArgumentException(message, paramName)
{
}
