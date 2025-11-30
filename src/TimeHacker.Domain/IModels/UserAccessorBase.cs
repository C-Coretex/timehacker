namespace TimeHacker.Domain.IModels;

public abstract class UserAccessorBase
{
    public Guid? UserId { get; protected set; }
    public bool IsUserValid;

    public Guid GetUserIdOrThrowUnauthorized() => UserId ?? throw new UnauthorizedAccessException();
}
