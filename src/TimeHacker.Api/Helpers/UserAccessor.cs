using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.Users;
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

            UserId = await GetOrCreateUserId(userIdentityId);

            if (UserId != null)
                session.Set(UserIdKey, UserId.Value.ToByteArray());
        }

        private async Task<Guid> GetOrCreateUserId(string userIdentityId)
        {
            var userId = await _userRepository.GetAll().Where(x => x.IdentityId == userIdentityId).Select(x => (Guid?)x.Id).FirstOrDefaultAsync();
            if (userId.HasValue)
                return userId.Value;

            var entity = new User
            {
                IdentityId = userIdentityId,
                Name = "New User"
            };
            entity = await _userRepository.AddAndSaveAsync(entity);
            return entity.Id;
        }

        public new bool IsUserValid => ValidateUser();

        protected bool ValidateUser()
        {
            return UserId.HasValue;
        }
    }
}
