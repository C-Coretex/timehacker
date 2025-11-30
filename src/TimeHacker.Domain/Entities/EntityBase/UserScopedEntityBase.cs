using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Domain.Abstractions.Classes;

namespace TimeHacker.Domain.Entities.EntityBase;

public class UserScopedEntityBase: GuidDbEntity, IUserScopedEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}
