using Microsoft.EntityFrameworkCore.Query;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Helpers.Db.Abstractions.BaseClasses;

/// <summary>
/// ExecuteUpdateAsync and ExecuteDeleteAsync should not be available for services. If you need them, extend specific repository with specific business logic method.
/// If you are using ExecuteUpdateAsync don't forget to manually set UpdatedTimestamp property, since SaveChangesAsync method is not being executed.
/// </summary>
public class RepositoryBase<TDbContext, TModel>(TDbContext dbContext, DbSet<TModel> dbSet, TimeProvider timeProvider) : IRepositoryBase<TModel>
    where TModel : class, IDbEntity
    where TDbContext : DbContextBase<TDbContext>
{
    // An expression selecting TModel.UpdatedTimestamp, used to stamp the timestamp during ExecuteUpdateAsync
    // (which bypasses SaveChangesAsync). Built once via reflection and cached; null when TModel isn't
    // IUpdatable, so callers skip the extra SetProperty entirely.
    private static readonly Lazy<Expression<Func<TModel, DateTime?>>?> UpdatedTimestampSelector = new(() =>
    {
        var isUpdatable = typeof(IUpdatable).IsAssignableFrom(typeof(TModel));
        if (!isUpdatable) return null;

        var param = Expression.Parameter(typeof(TModel), "x");
        var updatedProp = Expression.Property(param, nameof(IUpdatable.UpdatedTimestamp));
        return (Expression<Func<TModel, DateTime?>>)Expression.Lambda(updatedProp, param);
    });

    protected TDbContext DbContext { get; set; } = dbContext;
    protected DbSet<TModel> DbSet { get; set; } = dbSet;

    protected virtual IQueryable<TModel> GetAllBase() => DbSet;

    public virtual IQueryable<TModel> GetAll(params IEnumerable<QueryPipelineStep<TModel>> queryPipelineSteps) 
        => GetAll(true, queryPipelineSteps);

    public virtual IQueryable<TModel> GetAll(bool asNoTracking = true, params IEnumerable<QueryPipelineStep<TModel>> queryPipelineSteps)
    {
        ArgumentNullException.ThrowIfNull(queryPipelineSteps);

        var query = GetAllBase();
        if (asNoTracking)
            query = query.AsNoTracking();

        // Compose the query from caller-supplied steps (each an IQueryable->IQueryable transform, e.g. an
        // Include or filter), applied in order. Lets callers layer query shape without subclassing.
        foreach (var queryPipelineStep in queryPipelineSteps)
            query = queryPipelineStep(query);

        return query;
    }

    public virtual TModel Add(TModel model) 
        => DbContextBase<TDbContext>.AddEntity(DbSet, model);
    public virtual async Task<TModel> AddAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
    {
        var entity = Add(model);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual void AddRange(IEnumerable<TModel> models) 
        => DbContextBase<TDbContext>.AddEntities(DbSet, models);
    public virtual Task AddRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        AddRange(models);
        return SaveChangesAsync(cancellationToken);
    }

    public virtual void Delete(TModel model) 
        => DbContextBase<TDbContext>.RemoveEntity(DbSet, model);
    public virtual Task DeleteAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
    {
        Delete(model);
        return SaveChangesAsync(cancellationToken);
    }

    public virtual void DeleteRange(IEnumerable<TModel> models)
        => DbContextBase<TDbContext>.RemoveEntities(DbSet, models);
    public virtual Task DeleteRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        DeleteRange(models);
        return SaveChangesAsync(cancellationToken);
    }

    public virtual Task<int> DeleteBy(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
        => ExecuteDeleteAsync(predicate, cancellationToken);

    public virtual TModel Update(TModel model)
        => DbContextBase<TDbContext>.UpdateEntity(DbSet, model);
    public virtual async Task<TModel> UpdateAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
    {
        var entity = Update(model);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual void UpdateRange(IEnumerable<TModel> models)
        => DbContextBase<TDbContext>.UpdateEntities(DbSet, models);
    public virtual Task UpdateRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        UpdateRange(models);
        return SaveChangesAsync(cancellationToken);
    }

    public virtual Task UpdateProperty<TKey>(Expression<Func<TModel, bool>> predicate, Expression<Func<TModel, TKey>> propertySelector, TKey value, CancellationToken cancellationToken = default)
    {
        // ExecuteUpdateAsync runs SQL directly and skips SaveChangesAsync's auto-stamping, so append an
        // UpdatedTimestamp set here when the entity supports it (selector is null otherwise).
        var updatedTimestampSelector = UpdatedTimestampSelector.Value;
        if (updatedTimestampSelector == null)
            return ExecuteUpdateAsync(predicate, setPropertyCalls => setPropertyCalls.SetProperty(propertySelector, value), cancellationToken);
        
        return ExecuteUpdateAsync(predicate,
            setPropertyCalls => setPropertyCalls
                .SetProperty(propertySelector, value)
                .SetProperty(updatedTimestampSelector, timeProvider.GetUtcNow().UtcDateTime), 
            cancellationToken);
    }

    protected virtual Task<int> ExecuteUpdateAsync(
        Expression<Func<TModel, bool>> predicate, 
        Action<UpdateSettersBuilder<TModel>> updateSettersBuilder, 
        CancellationToken cancellationToken = default)
    {
        var query = GetAllBase().Where(predicate);
        return query.ExecuteUpdateAsync(updateSettersBuilder, cancellationToken);
    }

    protected virtual Task<int> ExecuteDeleteAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var query = GetAllBase().Where(predicate);
        return query.ExecuteDeleteAsync(cancellationToken);
    }

    /// <summary>
    /// Save changes from the whole Unit of Work (scoped DbContext).
    /// This method will save all the changes tracked in the current DbContext across all the repositories in the scope.
    /// </summary>
    public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => DbContext.SaveChangesAsync(cancellationToken);
}

public class RepositoryBase<TDbContext, TModel, TId>(TDbContext dbContext, DbSet<TModel> dbSet, TimeProvider timeProvider) 
    : RepositoryBase<TDbContext, TModel>(dbContext, dbSet, timeProvider), IRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>
    where TDbContext : DbContextBase<TDbContext>
{
    public virtual Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
        => GetAllBase().AnyAsync(x => x.Id!.Equals(id), cancellationToken);

    public virtual Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true, CancellationToken cancellationToken = default, params IEnumerable<QueryPipelineStep<TModel>> queryPipelineSteps)
        => GetAll(asNoTracking, queryPipelineSteps).FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);

    public virtual async Task<bool> DeleteAndSaveAsync(TId id, CancellationToken cancellationToken = default)
    {
        // Without the check the Delete returns success even if the entity doesn't exist or is filtered out by RLS
        if (!await ExistsAsync(id, cancellationToken))
            return false;

        await ExecuteDeleteAsync(x => x.Id!.Equals(id), cancellationToken);
        return true;
    }

    public virtual Task DeleteRangeAndSaveAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
        => ExecuteDeleteAsync(x => ids.Contains(x.Id), cancellationToken);

    public virtual async Task<TModel?> GetAndUpdateAndSaveAsync(TId id, Action<TModel> updateFunction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updateFunction);

        // Fetch tracked so the mutation is persisted by SaveChangesAsync and the original xmin concurrency token is carried.
        var entity = await GetByIdAsync(id, asNoTracking: false, cancellationToken: cancellationToken);
        if (entity is null)
            return null;

        updateFunction(entity);
        await SaveChangesAsync(cancellationToken);

        return entity;
    }
}
