using TimeHacker.Domain.Contracts.Entities.Tags;
using TimeHacker.Domain.Contracts.IRepositories.Tags;
using TimeHacker.Infrastructure.Repositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tags
{
    public class TagRepository: TaskRepository<Tag, Guid>, ITagRepository
    {
        public TagRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.Tag)
        { }
    }
}
