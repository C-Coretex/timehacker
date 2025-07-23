using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.Users
{
    public class User : GuidDbEntity
    {
        public string IdentityId { get; init; }

        public string Name { get; set; }
        public string? PhoneNumberForNotifications { get; set; }
        public string? EmailForNotifications { get; set; }
        public DateOnly Birthday { get; set; }
    }
}
