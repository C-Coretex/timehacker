using TimeHacker.Application.Api.Contracts.DTOs.Users;
using TimeHacker.Application.Api.Contracts.IAppServices.Users;
using TimeHacker.Domain.IRepositories.Users;

namespace TimeHacker.Application.Api.AppServices.Users;

public class UserService(IUserRepository userRepository, UserAccessorBase userAccessorBase)
    : IUserAppService
{
    public async Task<UserDto?> GetCurrent()
    {
        var userId = userAccessorBase.GetUserIdOrThrowUnauthorized();
        var entity = await userRepository.GetByIdAsync(userId);
        return UserDto.Create(entity);
    }

    public async Task UpdateAsync(UserDto user)
    {
        var userId = userAccessorBase.GetUserIdOrThrowUnauthorized();
        var userEntity = await userRepository.GetByIdAsync(userId) ?? throw new UserDoesNotExistException();
        userEntity = user.GetEntity(userEntity);

        await userRepository.UpdateAndSaveAsync(userEntity);
    }

    public async Task DeleteAsync()
    {
        var userId = userAccessorBase.GetUserIdOrThrowUnauthorized();
        await userRepository.DeleteAndSaveAsync(userId);
    }
}
