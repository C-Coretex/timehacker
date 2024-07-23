using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.IRepositories.Tasks
{
    public interface ITaskRepository<TTask, TId> : IRepositoryBase<TTask, TId>
        where TTask : class, IDbModel<TId>, new()
    {}
}
