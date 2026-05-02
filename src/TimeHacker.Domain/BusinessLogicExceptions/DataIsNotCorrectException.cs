namespace TimeHacker.Domain.BusinessLogicExceptions;

#pragma warning disable CA1032 // Implement standard exception constructors
public class DataIsNotCorrectException(string? message, string paramName) : ArgumentException(message, paramName)
#pragma warning restore CA1032 // Implement standard exception constructors
{
}
