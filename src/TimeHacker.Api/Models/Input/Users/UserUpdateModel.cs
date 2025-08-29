using System.ComponentModel.DataAnnotations;
using TimeHacker.Application.Api.Contracts.DTOs.Users;

namespace TimeHacker.Api.Models.Input.Users
{
    public record UserUpdateModel
    {
        [Required]
        public required string Name { get; init; }
        public string? PhoneNumberForNotifications { get; init; }
        public string? EmailForNotifications { get; init; }
        public DateOnly? Birthday { get; set; }

        public UserDto ToDto()
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
}
