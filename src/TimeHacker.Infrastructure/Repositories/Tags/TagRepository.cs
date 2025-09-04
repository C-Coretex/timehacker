using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Tags;

namespace TimeHacker.Infrastructure.Repositories.Tags
{
    public class TagRepository: UserScopedRepositoryBase<Tag, Guid>, ITagRepository
    {
        public TagRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) : base(dbContext, dbContext.Tag, userAccessor)
        { }
    }
}
