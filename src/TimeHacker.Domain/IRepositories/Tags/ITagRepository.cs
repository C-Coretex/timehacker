using TimeHacker.Domain.Entities.Tags;

namespace TimeHacker.Domain.IRepositories.Tags
{
    public interface ITagRepository : IUserScopedRepositoryBase<Tag, Guid>
    { }
}
