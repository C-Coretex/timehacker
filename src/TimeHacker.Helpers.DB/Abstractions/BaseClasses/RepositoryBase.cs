using Microsoft.EntityFrameworkCore;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Helpers.DB.Abstractions.BaseClasses
{
    public class RepositoryBase<TDbContext, TModel> : IRepositoryBase<TModel>
        where TModel : class, IDbModel, new()
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

        public virtual TModel Add(TModel model, bool saveChanges = true)
        {
            return _dbContext.AddEntity<TModel>(_dbSet, model, saveChanges);
        }
        public virtual Task<TModel> AddAsync(TModel model, bool saveChanges = true)
        {
            return _dbContext.AddEntityAsync<TModel>(_dbSet, model, saveChanges);
        }

        public virtual IEnumerable<TModel> AddRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return _dbContext.AddEntities<TModel>(_dbSet, models, saveChanges);
        }
        public virtual Task<IEnumerable<TModel>> AddRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return _dbContext.AddEntitiesAsync<TModel>(_dbSet, models, saveChanges);
        }

        public virtual void Delete(TModel model, bool saveChanges = true)
        {
            _dbContext.RemoveEntity<TModel>(_dbSet, model, saveChanges);
        }
        public virtual Task DeleteAsync(TModel model, bool saveChanges = true)
        {
            return _dbContext.RemoveEntityAsync<TModel>(_dbSet, model, saveChanges);
        }

        public virtual void DeleteRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            _dbContext.RemoveEntities<TModel>(_dbSet, models, saveChanges);
        }
        public virtual Task DeleteRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return _dbContext.RemoveEntitiesAsync<TModel>(_dbSet, models, saveChanges);
        }

        public virtual TModel Update(TModel model, bool saveChanges = true)
        {
            return _dbContext.UpdateEntity<TModel>(_dbSet, model, saveChanges);
        }
        public virtual Task<TModel> UpdateAsync(TModel model, bool saveChanges = true)
        {
            return _dbContext.UpdateEntityAsync<TModel>(_dbSet, model, saveChanges);
        }

        public virtual IEnumerable<TModel> UpdateRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return _dbContext.UpdateEntities<TModel>(_dbSet, models, saveChanges);
        }
        public virtual Task<IEnumerable<TModel>> UpdateRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return _dbContext.UpdateEntitiesAsync<TModel>(_dbSet, models, saveChanges);
        }

        public virtual void SaveChanges()
        {
            _dbContext.SaveDBChanges();
        }

        public virtual Task SaveChangesAsync(CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;
            return _dbContext.SaveDBChangesAsync(cancellationToken.Value);
        }
    }

    public class RepositoryBase<TDbContext, TModel, TId> : RepositoryBase<TDbContext, TModel>, IRepositoryBase<TModel, TId>
    where TModel : class, IDbModel<TId>, new()
    where TDbContext : DbContextBase<TDbContext>
    {
        public RepositoryBase(TDbContext dbContext, DbSet<TModel> dbSet) : base(dbContext, dbSet)
        { }

        public virtual TModel? GetById(TId id, bool asNoTracking = true, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates)
        {
            return GetAll(asNoTracking, includeExpansionDelegates).FirstOrDefault(x => x.Id!.Equals(id));
        }

        public virtual async Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates)
        {
            return await GetAll(asNoTracking, includeExpansionDelegates).FirstOrDefaultAsync(x => x.Id!.Equals(id));
        }

        public virtual void Delete(TId id, bool saveChanges = true)
        {
            _dbContext.RemoveEntity(_dbSet, id, saveChanges);
        }
        public virtual Task DeleteAsync(TId id, bool saveChanges = true)
        {
            return _dbContext.RemoveEntityAsync(_dbSet, id, saveChanges);
        }

        public virtual void DeleteRange(IEnumerable<TId> ids, bool saveChanges = true)
        {
            _dbContext.RemoveEntities(_dbSet, ids, saveChanges);
        }
        public virtual Task DeleteRangeAsync(IEnumerable<TId> ids, bool saveChanges = true)
        {
            return _dbContext.RemoveEntitiesAsync(_dbSet, ids, saveChanges);
        }
    }
}
