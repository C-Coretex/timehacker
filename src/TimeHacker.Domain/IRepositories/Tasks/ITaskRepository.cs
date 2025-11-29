using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Domain.IRepositories.Tasks
{
    //we are using base task repository, because dynamic and fixed tasks are basically the same thing in the end, and we may have the same methods for them
    public interface ITaskRepository<TTask, TId> : IUserScopedRepositoryBase<TTask, TId>
        where TTask : class, IDbEntity<TId>, IUserScopedEntity
    { }
}
