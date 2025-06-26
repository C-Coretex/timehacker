using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Domain.IRepositories.Tags
{
    public interface ITagRepository : ITaskRepository<Tag, Guid>
    { }
}
