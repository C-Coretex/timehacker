using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Domain.Entities.Tasks;

public interface ITask : IDbEntity<Guid>, IUserScopedEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public byte Priority { get; set; }
}
