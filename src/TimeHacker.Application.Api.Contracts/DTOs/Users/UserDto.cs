using TimeHacker.Domain.Entities.Users;

namespace TimeHacker.Application.Api.Contracts.DTOs.Users;

public record UserDto
{
    public Guid? Id { get; init; }
    public string Name { get; set; }
    public string? PhoneNumberForNotifications { get; set; }
    public string? EmailForNotifications { get; set; }
    public DateOnly? Birthday { get; set; }

    public static UserDto? Create(User? user)
    {
        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            PhoneNumberForNotifications = user.PhoneNumberForNotifications,
            EmailForNotifications = user.EmailForNotifications,
            Birthday = user.Birthday
        };
    }

    public User GetEntity(User? entity = null)
    {
        entity ??= new User();

        entity.Name = Name;
        entity.PhoneNumberForNotifications = PhoneNumberForNotifications;
        entity.EmailForNotifications = EmailForNotifications;
        entity.Birthday = Birthday;

        return entity;
    }
}
