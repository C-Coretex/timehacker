using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Domain.IRepositories
{
    //not inherited from IRepositoryBase, because some of the methods are changed to be Tasks, since they are async (e.g. Delete)
    public interface IUserScopedRepositoryBase<TModel, in TId> where TModel : class, IDbEntity<TId>, IUserScopedEntity
    {
        IQueryable<TModel> GetAll(params QueryPipelineStep<TModel>[] QueryPipelineSteps);
        IQueryable<TModel> GetAll(bool asNoTracking = true, params QueryPipelineStep<TModel>[] QueryPipelineSteps);
        TModel Add(TModel model);
        Task<TModel> AddAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
        void AddRange(IEnumerable<TModel> models);
        Task AddRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task Delete(TModel model, CancellationToken cancellationToken = default);
        Task DeleteAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
        Task DeleteRange(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task DeleteRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task<int> DeleteBy<TKey>(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TModel> Update(TModel model, CancellationToken cancellationToken = default);
        Task<TModel> UpdateAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
        Task UpdateRange(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task UpdateRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task UpdateProperty<TKey>(Expression<Func<TModel, bool>> predicate, Expression<Func<TModel, TKey>> propertySelector, TKey value, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
        Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true, CancellationToken cancellationToken = default, params QueryPipelineStep<TModel>[] queryPipelineSteps);

        Task DeleteAndSaveAsync(TId id, CancellationToken cancellationToken = default);
        Task DeleteRangeAndSaveAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
    }
}
