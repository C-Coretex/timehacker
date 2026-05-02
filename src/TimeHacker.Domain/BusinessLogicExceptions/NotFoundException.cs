namespace TimeHacker.Domain.BusinessLogicExceptions;

#pragma warning disable CA1032 // Implement standard exception constructors
public class NotFoundException(string resourceName, string resourceId) : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
{
    public string ResourceName { get; set; } = resourceName;
    public string ResourceId { get; set; } = resourceId;
}
