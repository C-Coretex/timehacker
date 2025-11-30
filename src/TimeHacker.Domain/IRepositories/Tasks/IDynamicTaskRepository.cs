using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.IRepositories.Tasks;

public interface IDynamicTaskRepository : ITaskRepository<DynamicTask, Guid>
{}
