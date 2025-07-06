using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Entities.Tasks
{
    public interface ITask : IDbEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public byte Priority { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
