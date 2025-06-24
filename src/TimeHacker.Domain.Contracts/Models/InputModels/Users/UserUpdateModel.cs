using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Domain.Contracts.Models.InputModels.Users
{
    public record UserUpdateModel
    {
        [Required]
        public required string Name { get; init; }
        public string? PhoneNumberForNotifications { get; init; }
        public string? EmailForNotifications { get; init; }
    }
}
