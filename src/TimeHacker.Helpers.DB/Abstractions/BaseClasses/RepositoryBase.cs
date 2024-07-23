using Helpers.DB.Abstractions.Classes;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Helpers.DB.Abstractions.BaseClasses
{
    public class RepositoryBase<TDbContext, TModel, TId> : IRepositoryBase<TModel, TId>
        where TModel : class, IDbModel<TId>, new()
        where TDbContext : DbContextBase<TDbContext>
    {
        protected TDbContext dbContext;
        protected DbSet<TModel> dbSet;

        public RepositoryBase(TDbContext dbContext, DbSet<TModel> dbSet)
        {
            this.dbContext = dbContext;
            this.dbSet = dbSet;
        }

        public virtual IQueryable<TModel> GetAll(bool asNoTracking = true)
        {
            var query = dbSet.AsQueryable();
            if (asNoTracking)
                query = query.AsNoTracking();
            return query;
        }

        public virtual TModel? GetById(TId id, bool asNoTracking = true)
        {
            return GetAll(asNoTracking).FirstOrDefault(x => x.Id!.Equals(id));
        }

        public virtual async Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true)
        {
            return await GetAll(asNoTracking).FirstOrDefaultAsync(x => x.Id!.Equals(id));
        }

        public virtual TModel Add(TModel model, bool saveChanges = true)
        {
            return dbContext.AddEntity<TModel, TId>(dbSet, model, saveChanges);
        }
        public virtual Task<TModel> AddAsync(TModel model, bool saveChanges = true)
        {
            return dbContext.AddEntityAsync<TModel, TId>(dbSet, model, saveChanges);
        }

        public virtual IEnumerable<TModel> AddRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.AddEntities<TModel, TId>(dbSet, models, saveChanges);
        }
        public virtual Task<IEnumerable<TModel>> AddRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.AddEntitiesAsync<TModel, TId>(dbSet, models, saveChanges);
        }

        public virtual void Delete(TModel model, bool saveChanges = true)
        {
            dbContext.RemoveEntity<TModel, TId>(dbSet, model, saveChanges);
        }
        public virtual Task DeleteAsync(TModel model, bool saveChanges = true)
        {
            return dbContext.RemoveEntityAsync<TModel, TId>(dbSet, model, saveChanges);
        }

        public virtual void DeleteRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            dbContext.RemoveEntities<TModel, TId>(dbSet, models, saveChanges);
        }
        public virtual Task DeleteRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.RemoveEntitiesAsync<TModel, TId>(dbSet, models, saveChanges);
        }

        public virtual void Delete(TId id, bool saveChanges = true)
        {
            dbContext.RemoveEntity(dbSet, id, saveChanges);
        }
        public virtual Task DeleteAsync(TId id, bool saveChanges = true)
        {
            return dbContext.RemoveEntityAsync(dbSet, id, saveChanges);
        }

        public virtual void DeleteRange(IEnumerable<TId> ids, bool saveChanges = true)
        {
            dbContext.RemoveEntities(dbSet, ids, saveChanges);
        }
        public virtual Task DeleteRangeAsync(IEnumerable<TId> ids, bool saveChanges = true)
        {
            return dbContext.RemoveEntitiesAsync(dbSet, ids, saveChanges);
        }

        public virtual TModel Update(TModel model, bool saveChanges = true)
        {
            return dbContext.UpdateEntity<TModel, TId>(dbSet, model, saveChanges);
        }
        public virtual Task<TModel> UpdateAsync(TModel model, bool saveChanges = true)
        {
            return dbContext.UpdateEntityAsync<TModel, TId>(dbSet, model, saveChanges);
        }

        public virtual IEnumerable<TModel> UpdateRange(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.UpdateEntities<TModel, TId>(dbSet, models, saveChanges);
        }
        public virtual Task<IEnumerable<TModel>> UpdateRangeAsync(IEnumerable<TModel> models, bool saveChanges = true)
        {
            return dbContext.UpdateEntitiesAsync<TModel, TId>(dbSet, models, saveChanges);
        }

        public virtual void SaveChanges()
        {
            dbContext.SaveDBChanges();
        }

        public virtual Task SaveChangesAsync(CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;
            return dbContext.SaveDBChangesAsync(cancellationToken.Value);
        }
    }
}
