using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Helpers.Db.Abstractions.BaseClasses
{
    /// <summary>
    /// ExecuteUpdateAsync and ExecuteDeleteAsync should not be available for services. If you need them, extend specific repository with specific business logic method.
    /// If you are using ExecuteUpdateAsync don't forget to manually set UpdatedTimestamp property, since SaveChangesAsync method is not being executed.
    /// </summary>
    public class RepositoryBase<TDbContext, TModel>(TDbContext dbContext, DbSet<TModel> dbSet) : IRepositoryBase<TModel>
        where TModel : class, IDbEntity
        where TDbContext : DbContextBase<TDbContext>
    {
        protected TDbContext DbContext = dbContext;
        protected DbSet<TModel> DbSet = dbSet;

        protected virtual IQueryable<TModel> GetAllBase()
        {
            return DbSet;
        }

        public virtual IQueryable<TModel> GetAll(params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates) => GetAll(true, includeExpansionDelegates);

        public virtual IQueryable<TModel> GetAll(bool asNoTracking = true, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates)
        {
            var query = GetAllBase();
            if (asNoTracking)
                query = query.AsNoTracking();

            foreach (var includeExpansionDelegate in includeExpansionDelegates)
                query = includeExpansionDelegate(query);

            return query;
        }

        public virtual TModel Add(TModel model)
        {
            return DbContext.AddEntity(DbSet, model);
        }
        public virtual async Task<TModel> AddAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            var entity = Add(model);
            await SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual void AddRange(IEnumerable<TModel> models)
        {
            DbContext.AddEntities(DbSet, models);
        }
        public virtual Task AddRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
        {
            AddRange(models);
            return SaveChangesAsync(cancellationToken);
        }

        public virtual void Delete(TModel model)
        {
            DbContext.RemoveEntity(DbSet, model);
        }
        public virtual Task DeleteAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            Delete(model);
            return SaveChangesAsync(cancellationToken);
        }

        public virtual void DeleteRange(IEnumerable<TModel> models)
        {
            DbContext.RemoveEntities(DbSet, models);
        }
        public virtual Task DeleteRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
        {
            DeleteRange(models);
            return SaveChangesAsync(cancellationToken);
        }

        public virtual TModel Update(TModel model)
        {
            return DbContext.UpdateEntity(DbSet, model);
        }
        public virtual async Task<TModel> UpdateAndSaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            var entity = Update(model);
            await SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual void UpdateRange(IEnumerable<TModel> models)
        {
            DbContext.UpdateEntities(DbSet, models);
        }
        public virtual Task UpdateRangeAndSaveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
        {
            UpdateRange(models);
            return SaveChangesAsync(cancellationToken);
        }

        protected virtual Task<int> ExecuteUpdateAsync(Expression<Func<TModel, bool>> predicate, Expression<Func<SetPropertyCalls<TModel>, SetPropertyCalls<TModel>>> setPropertyCalls, CancellationToken cancellationToken = default)
        {
            var query = GetAllBase().Where(predicate);
            return query.ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
        }

        protected virtual Task<int> ExecuteDeleteAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken)
        {
            var query = GetAllBase().Where(predicate);
            return query.ExecuteDeleteAsync(cancellationToken);
        }

        public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var createdEntries = DbContext.ChangeTracker.Entries<ICreatable>().Where(entry => entry.State == EntityState.Added);
            foreach (var entry in createdEntries)
                entry.Entity.CreatedTimestamp = now;

            var updatedEntries = DbContext.ChangeTracker.Entries<IUpdatable>().Where(entry => entry.State == EntityState.Modified);
            foreach (var entry in updatedEntries)
                entry.Entity.UpdatedTimestamp = now;

            return DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public class RepositoryBase<TDbContext, TModel, TId> : RepositoryBase<TDbContext, TModel>, IRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>
    where TDbContext : DbContextBase<TDbContext>
    {
        public RepositoryBase(TDbContext dbContext, DbSet<TModel> dbSet) : base(dbContext, dbSet)
        { }

        public virtual Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
        {
            return GetAllBase().AnyAsync(x => x.Id!.Equals(id), cancellationToken);
        }

        public virtual async Task<TModel?> GetByIdAsync(TId id, bool asNoTracking = true, CancellationToken cancellationToken = default, params IncludeExpansionDelegate<TModel>[] includeExpansionDelegates)
        {
            return await GetAll(asNoTracking, includeExpansionDelegates).FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);
        }


        public virtual Task DeleteAndSaveAsync(TId id, CancellationToken cancellationToken = default)
        {
            return ExecuteDeleteAsync(x => x.Id!.Equals(id), cancellationToken);
        }
        public virtual Task DeleteRangeAndSaveAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
        {
            return ExecuteDeleteAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}
