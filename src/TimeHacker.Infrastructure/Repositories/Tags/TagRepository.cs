using TimeHacker.Domain.Contracts.Entities.Tags;
using TimeHacker.Domain.Contracts.IRepositories.Tags;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.Tags
{
    public class TagRepository: RepositoryBase<TimeHackerDbContext, Tag, Guid>, ITagRepository
    {
        public TagRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.Tag)
        { }
    }
}
