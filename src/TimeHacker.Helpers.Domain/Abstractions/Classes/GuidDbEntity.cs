using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Helpers.Domain.Abstractions.Classes
{
    public abstract class GuidDbEntity : IDbEntity<Guid>
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();
    }
}
