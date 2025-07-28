using TimeHacker.Application.Api.Contracts.IAppServices.Users;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Users;
using TimeHacker.Domain.Models.InputModels.Users;

namespace TimeHacker.Application.Api.AppServices.Users
{
    public class UserService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserAccessorBase _userAccessorBase;

        public UserService(IUserRepository userRepository, UserAccessorBase userAccessorBase)
        {
            _userRepository = userRepository;
            _userAccessorBase = userAccessorBase;
        }

        public async Task AddAsync(UserUpdateModel user, string userIdentityId)
        {
            var userEntity = new User()
            {
                IdentityId = userIdentityId,
                Name = user.Name,
                PhoneNumberForNotifications = user.PhoneNumberForNotifications,
                EmailForNotifications = user.EmailForNotifications
            };

            await _userRepository.AddAndSaveAsync(userEntity);
        }

        public Task<User?> GetCurrent()
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            return _userRepository.GetByIdAsync(userId);
        }

        public async Task UpdateAsync(UserUpdateModel user)
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            var userEntity = await _userRepository.GetByIdAsync(userId) ?? throw new UserDoesNotExistException();

            userEntity.Name = user.Name;
            userEntity.PhoneNumberForNotifications = user.PhoneNumberForNotifications;
            userEntity.EmailForNotifications = user.EmailForNotifications;

            await _userRepository.UpdateAndSaveAsync(userEntity);
        }

        public async Task DeleteAsync()
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            await _userRepository.DeleteAndSaveAsync(userId);
        }
    }
}
