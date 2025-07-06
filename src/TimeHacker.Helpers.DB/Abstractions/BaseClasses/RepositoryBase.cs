using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Helpers.Db.Abstractions.BaseClasses
{
    public class RepositoryBase<TDbContext, TModel> : IRepositoryBase<TModel>
        where TModel : class, IDbEntity, new()
        where TDbContext : DbContextBase<TDbContext>
    {
        protected TDbContext _dbContext;
        protected DbSet<TModel> _dbSet;

        public RepositoryBase(TDbContext dbContext, DbSet<TModel> dbSet)
        {
            _dbContext = dbContext;
            _dbSet = dbSet;
        }

        public virtual IQueryable<TModel> GetAll(params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates) => GetAll(true, includeExpansionDelegates);

        public virtual IQueryable<TModel> GetAll(bool asNoTracking = true, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates)
        {
            var query = _dbSet.AsQueryable();
            if (asNoTracking)
                query = query.AsNoTracking();

            foreach (var includeExpansionDelegate in includeExpansionDelegates)
                query = includeExpansionDelegate(query);

            return query;
        }

        public virtual TModel Add(TModel model)
        {
            return _dbContext.AddEntity(_dbSet, model);
        }
        public virtual Task<TModel> AddAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            return _dbContext.AddEntityAndSaveAsync(_dbSet, model, cancellationToken);
        }

        public virtual void AddRange(IEnumerable<TModel> models)
        {
            _dbContext.AddEntities(_dbSet, models);
        }
        public virtual Task AddRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
        {
            return _dbContext.AddEntitiesAndSaveAsync(_dbSet, models, cancellationToken);
        }

        public virtual void Delete(TModel model)
        {
            _dbContext.RemoveEntity(_dbSet, model);
        }
        public virtual Task DeleteAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            return _dbContext.RemoveEntityAndSaveAsync(_dbSet, model, cancellationToken);
        }

        public virtual void DeleteRange(IEnumerable<TModel> models)
        {
            _dbContext.RemoveEntities(_dbSet, models);
        }
        public virtual Task DeleteRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
        {
            return _dbContext.RemoveEntitiesAndSaveAsync(_dbSet, models, cancellationToken);
        }

        public virtual TModel Update(TModel model)
        {
            return _dbContext.UpdateEntity<TModel>(_dbSet, model);
        }
        public virtual Task<TModel> UpdateAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            return _dbContext.UpdateEntityAndSaveAsync(_dbSet, model, cancellationToken);
        }

        public virtual void UpdateRange(IEnumerable<TModel> models)
        {
            _dbContext.UpdateEntities(_dbSet, models);
        }
        public virtual Task UpdateRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
        {
            return _dbContext.UpdateEntitiesAndSaveAsync(_dbSet, models, cancellationToken);
        }

        public virtual Task<int> ExecuteUpdateAsync(Expression<Func<TModel, bool>> predicate, Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls, CancellationToken cancellationToken = default)
        {
            var query = GetAll(asNoTracking: false).Where(predicate);
            return query.ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
        }

        public virtual Task<int> ExecuteDeleteAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken)
        {
            var query = GetAll(asNoTracking: false).Where(predicate);
            return query.ExecuteDeleteAsync(cancellationToken);
        }

        public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public class RepositoryBase<TDbContext, TModel, TId> : RepositoryBase<TDbContext, TModel>, IRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>, new()
    where TDbContext : DbContextBase<TDbContext>
    {
        public RepositoryBase(TDbContext dbContext, DbSet<TModel> dbSet) : base(dbContext, dbSet)
        { }

        public virtual Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
        {
            return GetAll().AnyAsync(x => x.Id!.Equals(id), cancellationToken);
        }

        public virtual async Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true, CancellationToken cancellationToken = default, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates)
        {
            return await GetAll(asNoTracking, includeExpansionDelegates).FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);
        }

        public virtual void Delete(TId id)
        {
            _dbContext.RemoveEntity(_dbSet, id);
        }
        public virtual Task DeleteAndSaveAsync(TId id, CancellationToken cancellationToken = default)
        {
            return _dbContext.RemoveEntityAndSaveAsync(_dbSet, id, cancellationToken);
        }

        public virtual void DeleteRange(IEnumerable<TId> ids)
        {
            _dbContext.RemoveEntities(_dbSet, ids);
        }
        public virtual Task DeleteRangeAndSaveAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
        {
            return _dbContext.RemoveEntitiesAndSaveAsync(_dbSet, ids, cancellationToken);
        }
    }
}
