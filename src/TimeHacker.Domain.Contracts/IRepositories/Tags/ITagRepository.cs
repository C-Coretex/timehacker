using TimeHacker.Domain.Contracts.Entities.Tags;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;

namespace TimeHacker.Domain.Contracts.IRepositories.Tags
{
    public interface ITagRepository : ITaskRepository<Tag, Guid>
    { }
}
