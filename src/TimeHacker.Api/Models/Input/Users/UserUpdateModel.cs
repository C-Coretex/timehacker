using TimeHacker.Application.Api.Contracts.DTOs.Users;

namespace TimeHacker.Api.Models.Input.Users;

public record UserUpdateModel
{
    [Required]
    [StringLength(64, MinimumLength = 1)]
    public required string Name { get; init; }

    [Phone]
    public string? PhoneNumberForNotifications { get; init; }

    [EmailAddress]
    public string? EmailForNotifications { get; init; }

    public DateOnly? Birthday { get; set; }

    public UserDto CreateDto()
    {
        return new UserDto
        {
            Name = Name,
            PhoneNumberForNotifications = PhoneNumberForNotifications,
            EmailForNotifications = EmailForNotifications,
            Birthday = Birthday
        };
    }
}
