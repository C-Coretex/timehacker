using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.IRepositories.Tags;

namespace TimeHacker.Infrastructure.Repositories.Tags;

internal sealed class TagRepository: UserScopedRepositoryBase<Tag, Guid>, ITagRepository
{
    public TagRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor)
        : base(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.Tag, userAccessor)
    { }
}
