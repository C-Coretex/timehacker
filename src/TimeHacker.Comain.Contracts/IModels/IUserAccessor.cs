namespace TimeHacker.Domain.Contracts.IModels
{
    public class IUserAccessor
    {
        public string? UserId { get; init; }
        public bool IsUserValid;
    }
}
