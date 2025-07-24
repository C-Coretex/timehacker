namespace TimeHacker.Domain.Entities.EntityBase
{
    public interface IUserScopedEntity
    {
        //UserScopedRepository
        //change UserIdentityId to something different

        public Guid UserId { get; set; }
    }
}
