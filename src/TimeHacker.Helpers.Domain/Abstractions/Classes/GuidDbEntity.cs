namespace TimeHacker.Helpers.Domain.Abstractions.Classes
{
    public abstract class GuidDbEntity : IDbEntity<Guid>, ICreatable, IUpdatable
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();

        public DateTime CreatedTimestamp { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
    }
}
