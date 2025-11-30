namespace TimeHacker.Domain.Entities.EntityBase;

public interface IUserScopedEntity
{
    public Guid UserId { get; set; }
}
