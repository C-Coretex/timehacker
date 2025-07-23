using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.IRepositories.Tasks
{
    public interface IFixedTaskRepository : ITaskRepository<FixedTask, Guid>
    {}
}
