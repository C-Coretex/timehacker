using System.Security.Claims;
using TimeHacker.Domain.Abstractions.Interfaces.Helpers;

namespace TimeHacker.Application.Helpers
{
    public class UserAccessor: IUserAccessor
    {
        public string? UserId { get; init; }
        public bool IsUserValid => ValidateUser();
        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        protected bool ValidateUser()
        {
            return !string.IsNullOrWhiteSpace(UserId);
        }
    }
}
