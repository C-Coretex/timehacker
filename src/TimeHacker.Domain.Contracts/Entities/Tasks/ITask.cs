using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public interface ITask : IDbModel<uint>
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public uint Priority { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
