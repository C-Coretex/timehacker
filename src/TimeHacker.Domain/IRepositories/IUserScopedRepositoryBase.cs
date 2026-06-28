using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Domain.IRepositories;

//not inherited from IRepositoryBase, because some of the methods are changed to be Tasks, since they are async (e.g. Delete)
public interface IUserScopedRepositoryBase<TModel, in TId> : IRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>, IUserScopedEntity
{
}
