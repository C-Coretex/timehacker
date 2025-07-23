using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Domain.IRepositories.Tasks
{
    public interface ITaskRepository<TTask, TId> : IRepositoryBase<TTask, TId>
        where TTask : class, IDbEntity<TId>, new()
    {}
}
