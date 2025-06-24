using AutoMapper;
using TimeHacker.Domain.Contracts.BusinessLogicExceptions;
using TimeHacker.Domain.Contracts.Entities.Users;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Users;
using TimeHacker.Domain.Contracts.IServices.Users;
using TimeHacker.Domain.Contracts.Models.InputModels.Users;

namespace TimeHacker.Domain.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserAccessorBase _userAccessorBase;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, UserAccessorBase userAccessorBase, IMapper mapper)
        {
            _userRepository = userRepository;
            _userAccessorBase = userAccessorBase;
            _mapper = mapper;
        }

        public async Task AddAsync(UserUpdateModel user)
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            if (await _userRepository.ExistsAsync(userId))
                throw new UserAlreadyPresentException();

            var userEntity = new User()
            {
                Id = userId
            };
            userEntity = _mapper.Map(user, userEntity);

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

            userEntity = _mapper.Map(user, userEntity);

            await _userRepository.UpdateAndSaveAsync(userEntity);
        }

        public async Task DeleteAsync()
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            await _userRepository.DeleteAndSaveAsync(userId);
        }
    }
}
