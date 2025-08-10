using TimeHacker.Application.Api.Contracts.DTOs.Users;

namespace TimeHacker.Api.Models.Return.Users
{
    public record UserReturnModel(
        string Name,
        string? PhoneNumberForNotifications,
        string? EmailForNotifications
    )
    {
        public static UserReturnModel Create(UserDto user)
        {
            return new UserReturnModel(
                user.Name,
                user.PhoneNumberForNotifications,
                user.EmailForNotifications
            );
        }
    }
}
