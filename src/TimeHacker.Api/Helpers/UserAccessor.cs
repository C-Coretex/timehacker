using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.IRepositories.Users;

namespace TimeHacker.Api.Helpers;

internal sealed class UserAccessor : UserAccessorBase
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string UserIdKey = "UserIdKey";
    public UserAccessor(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;

        // Restore the cached domain UserId from session (stored as the Guid's 16 raw bytes). The length
        // check defends against corrupt/legacy session data; on mismatch we drop it so Init() rebuilds it.
        var session = httpContextAccessor.HttpContext?.Session;
        if (session?.TryGetValue(UserIdKey, out var bytes) != true)
            return;

        if(bytes!.Length == 16)
            UserId = new Guid(bytes);
        else
            session.Remove(UserIdKey);
    }

   
    /// <summary>
    /// Resolves the current request's domain UserId once and caches it in session. Bridges ASP.NET Identity
    /// (the NameIdentifier claim) to the domain User, creating one on first login. No-op once resolved.
    /// </summary>
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
        session.Set(UserIdKey, UserId.Value.ToByteArray());
    }

    private async Task<Guid> GetOrCreateUserId(string userIdentityId)
    {
        var userId = await _userRepository.GetAll().Where(x => x.IdentityId == userIdentityId).Select(x => (Guid?)x.Id).FirstOrDefaultAsync();
        if (userId.HasValue)
            return userId.Value;

        // First request after registration: provision the domain User with a placeholder name the user
        // can later edit on their profile.
        var entity = new User
        {
            IdentityId = userIdentityId,
            Name = "New User"
        };
        entity = await _userRepository.AddAndSaveAsync(entity);
        return entity.Id;
    }

    public new bool IsUserValid => ValidateUser();

    private bool ValidateUser()
    {
        return UserId.HasValue;
    }
}
