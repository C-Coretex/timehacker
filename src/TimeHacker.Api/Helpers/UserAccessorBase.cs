using System.Security.Claims;
using TimeHacker.Domain.IModels;

namespace TimeHacker.Api.Helpers
{
    public class UserAccessor: UserAccessorBase
    {
        public new bool IsUserValid => ValidateUser();
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
