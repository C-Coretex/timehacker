using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.IRepositories.Tasks
{
    public interface IFixedTaskRepository : ITaskRepository<FixedTask, int>
    {}
}
