namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces;

//for composite primary keys
public interface IRepositoryBase<TModel> where TModel : class, IDbEntity
{
    IQueryable<TModel> GetAll(params QueryPipelineStep<TModel>[] queryPipelineSteps);
    IQueryable<TModel> GetAll(bool asNoTracking = true, params QueryPipelineStep<TModel>[] queryPipelineSteps);
    TModel Add(TModel model);
    Task<TModel> AddAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
    void AddRange(IEnumerable<TModel> models);
    Task AddRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
    void Delete(TModel model);
    Task DeleteAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
    void DeleteRange(IEnumerable<TModel> models);
    Task DeleteRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
    Task<int> DeleteBy<TKey>(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
    TModel Update(TModel model);
    Task<TModel> UpdateAndSaveAsync(TModel model, CancellationToken cancellationToken = default);
    void UpdateRange(IEnumerable<TModel> models);
    Task UpdateRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
    Task UpdateProperty<TKey>(Expression<Func<TModel, bool>> predicate, Expression<Func<TModel, TKey>> propertySelector, TKey value, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IRepositoryBase<TModel, in TId>: IRepositoryBase<TModel> where TModel : class, IDbEntity<TId>
{
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true, CancellationToken cancellationToken = default, params QueryPipelineStep<TModel>[] queryPipelineSteps);

    Task DeleteAndSaveAsync(TId id, CancellationToken cancellationToken = default);
    Task DeleteRangeAndSaveAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
}
