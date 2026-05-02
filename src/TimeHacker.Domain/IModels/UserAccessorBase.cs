namespace TimeHacker.Domain.IModels;

public abstract class UserAccessorBase
{
    public Guid? UserId { get; protected set; }
    public bool IsUserValid { get; set; }

#pragma warning disable CA1024 // Use properties where appropriate
    public Guid GetUserIdOrThrowUnauthorized() => UserId ?? throw new UnauthorizedAccessException();
#pragma warning restore CA1024 // Use properties where appropriate
}
