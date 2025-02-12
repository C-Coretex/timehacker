namespace TimeHacker.Domain.Contracts.IModels
{
    public class UserAccessorBase
    {
        public string? UserId { get; init; }
        public bool IsUserValid;

        public string GetUserIdOrThrowUnauthorized() => UserId ?? throw new UnauthorizedAccessException();
    }
}
