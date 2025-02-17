using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.Users
{
    public class User : IDbModel<string>
    {
        public string Id { get; init; }

        public string Name { get; set; }
        public string PhoneNumberForNotifications { get; set; }
        public string EmailForNotifications { get; set; }
        public DateOnly Birthday { get; set; }
    }
}
