using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.IRepositories.Tags;

namespace TimeHacker.Infrastructure.Repositories.Tags;

internal sealed class TagRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : UserScopedRepositoryBase<Tag, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.Tag, userAccessor, timeProvider), ITagRepository
{
}
