using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.IRepositories.Tasks
{
    public interface IDynamicTaskRepository : ITaskRepository<DynamicTask, Guid>
    {}
}
