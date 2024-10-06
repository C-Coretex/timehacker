using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public interface ITask : IDbModel<Guid>
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public byte Priority { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
