using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces
{
    public interface IRepositoryBase<TModel> where TModel : class, IDbEntity, new()
    {
        IQueryable<TModel> GetAll(params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates);
        IQueryable<TModel> GetAll(bool asNoTracking = true, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates);
        TModel Add(TModel model);
        Task<TModel> AddAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
        void AddRange(IEnumerable<TModel> models);
        Task AddRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        void Delete(TModel model);
        Task DeleteAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
        void DeleteRange(IEnumerable<TModel> models);
        Task DeleteRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        TModel Update(TModel model);
        Task<TModel> UpdateAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
        void UpdateRange(IEnumerable<TModel> models);
        Task UpdateRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task<int> ExecuteUpdateAsync(Expression<Func<TModel, bool>> predicate, Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IRepositoryBase<TModel, TId>: IRepositoryBase<TModel> where TModel : class, IDbEntity<TId>, new()
    {
        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
        Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true, CancellationToken cancellationToken = default, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates);
        void Delete(TId id);
        Task DeleteAndSaveAsync(TId id, CancellationToken cancellationToken = default);
        void DeleteRange(IEnumerable<TId> ids);
        Task DeleteRangeAndSaveAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
    }
}
