using TimeHacker.Domain.Entities.Users;

namespace TimeHacker.Api.Models.Return.Users
{
    public record UserReturnModel(
        string Name,
        string? PhoneNumberForNotifications,
        string? EmailForNotifications
    )
    {
        public static UserReturnModel Create(User user)
        {
            return new UserReturnModel(
                user.Name,
                user.PhoneNumberForNotifications,
                user.EmailForNotifications
            );
        }
    }
}
