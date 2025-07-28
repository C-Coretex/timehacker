using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Users;

namespace TimeHacker.Api.Helpers
{
    public class UserAccessor : UserAccessorBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string UserIdKey = "UserIdKey";
        public UserAccessor(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;

            var session = httpContextAccessor.HttpContext?.Session;
            if (session?.TryGetValue(UserIdKey, out var bytes) != true)
                return;

            if(bytes!.Length == 16)
                UserId = new Guid(bytes);
            else
                session.Remove(UserIdKey);
        }

       
        public async Task Init()
        {
            if (IsUserValid)
                return;

            var context = _httpContextAccessor.HttpContext;
            var userIdentityId = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var session = context?.Session;

            if (session == null || string.IsNullOrWhiteSpace(userIdentityId))
                return;

            UserId = (await _userRepository.GetAll().FirstOrDefaultAsync(x => x.IdentityId == userIdentityId))?.Id;

            if (UserId != null)
                session.Set(UserIdKey, UserId.Value.ToByteArray());
        }

        public new bool IsUserValid => ValidateUser();

        protected bool ValidateUser()
        {
            return UserId.HasValue;
        }
    }
}
