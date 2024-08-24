using Microsoft.EntityFrameworkCore;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Helpers.DB.Abstractions.BaseClasses
{
    public class DbContextBase<TContext> : DbContext where TContext : DbContext
    {
        private readonly string? _connectionString = null;
        public DbContextBase(DbContextOptions<TContext> options) : base(options)
        { }
        public DbContextBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
                optionsBuilder.UseSqlServer(_connectionString);
        }

        internal T AddEntity<T>(DbSet<T> entityCollection, T entity, bool saveChanges = true) where T : class, IDbModel
        {
            var newEntity = entityCollection.Add(entity).Entity;
            if (saveChanges)
                SaveChanges();

            return newEntity;
        }
        internal async Task<T> AddEntityAsync<T>(DbSet<T> entityCollection, T entity, bool saveChanges = true) where T : class, IDbModel
        {
            var newEntity = await entityCollection.AddAsync(entity);
            if (saveChanges)
                await SaveChangesAsync();

            return newEntity.Entity;
        }
        internal IEnumerable<T> AddEntities<T>(DbSet<T> entityCollection, IEnumerable<T> entities, bool saveChanges = true) where T : class, IDbModel
        {
            entityCollection.AddRange(entities);
            if (saveChanges)
                SaveChanges();

            return entities;
        }
        internal async Task<IEnumerable<T>> AddEntitiesAsync<T>(DbSet<T> entityCollection, IEnumerable<T> entities, bool saveChanges = true) where T : class, IDbModel
        {
            await entityCollection.AddRangeAsync(entities);
            if (saveChanges)
                await SaveChangesAsync();

            return entities;
        }

        internal T UpdateEntity<T>(DbSet<T> entityCollection, T entity, bool saveChanges = true) where T : class, IDbModel
        {
            var newEntity = entityCollection.Update(entity).Entity;
            if (saveChanges)
                SaveChanges();

            return newEntity;
        }
        internal async Task<T> UpdateEntityAsync<T>(DbSet<T> entityCollection, T entity, bool saveChanges = true) where T : class, IDbModel
        {
            var newEntity = entityCollection.Update(entity).Entity;
            if (saveChanges)
                await SaveChangesAsync();

            return newEntity;
        }
        internal IEnumerable<T> UpdateEntities<T>(DbSet<T> entityCollection, IEnumerable<T> entities, bool saveChanges = true) where T : class, IDbModel
        {
            entityCollection.UpdateRange(entities);
            if (saveChanges)
                SaveChanges();

            return entities;
        }
        internal async Task<IEnumerable<T>> UpdateEntitiesAsync<T>(DbSet<T> entityCollection, IEnumerable<T> entities, bool saveChanges = true) where T : class, IDbModel
        {
            entityCollection.UpdateRange(entities);
            if (saveChanges)
                await SaveChangesAsync();

            return entities;
        }

        internal void RemoveEntity<T>(DbSet<T> entityCollection, T model, bool saveChanges = true) where T : class, IDbModel, new()
        {
            entityCollection.Remove(model);
            if (saveChanges)
                SaveChanges();
        }
        internal async Task RemoveEntityAsync<T>(DbSet<T> entityCollection, T model, bool saveChanges = true) where T : class, IDbModel, new()
        {
            entityCollection.Remove(model);
            if (saveChanges)
                await SaveChangesAsync();
        }
        internal void RemoveEntities<T>(DbSet<T> entityCollection, IEnumerable<T> models, bool saveChanges = true) where T : class, IDbModel, new()
        {
            entityCollection.RemoveRange(models);
            if (saveChanges)
                SaveChanges();
        }
        internal async Task RemoveEntitiesAsync<T>(DbSet<T> entityCollection, IEnumerable<T> models, bool saveChanges = true) where T : class, IDbModel, new()
        {
            entityCollection.RemoveRange(models);
            if (saveChanges)
                await SaveChangesAsync();
        }

        internal void RemoveEntity<T, TId>(DbSet<T> entityCollection, TId id, bool saveChanges = true) where T : class, IDbModel<TId>, new()
        {
            entityCollection.Remove(new T() { Id = id });
            if (saveChanges)
                SaveChanges();
        }
        internal async Task RemoveEntityAsync<T, TId>(DbSet<T> entityCollection, TId id, bool saveChanges = true) where T : class, IDbModel<TId>, new()
        {
            entityCollection.Remove(new T() { Id = id });
            if (saveChanges)
                await SaveChangesAsync();
        }
        internal void RemoveEntities<T, TId>(DbSet<T> entityCollection, IEnumerable<TId> ids, bool saveChanges = true) where T : class, IDbModel<TId>, new()
        {
            entityCollection.RemoveRange(ids.Select(id => new T() { Id = id }));
            if (saveChanges)
                SaveChanges();
        }
        internal async Task RemoveEntitiesAsync<T, TId>(DbSet<T> entityCollection, IEnumerable<TId> ids, bool saveChanges = true) where T : class, IDbModel<TId>, new()
        {
            entityCollection.RemoveRange(ids.Select(id => new T() { Id = id }));
            if (saveChanges)
                await SaveChangesAsync();
        }

        internal void RemoveEntities<T>(DbSet<T> entityCollection, Func<T, bool> selector, bool saveChanges = true) where T : class, IDbModel
        {
            var entitiesToRemove = entityCollection.Where(selector);
            entityCollection.RemoveRange(entitiesToRemove);
            if (saveChanges)
                SaveChanges();
        }
        internal async Task RemoveEntitiesAsync<T>(DbSet<T> entityCollection, Func<T, bool> selector, bool saveChanges = true) where T : class, IDbModel
        {
            var entitiesToRemove = entityCollection.Where(selector);
            entityCollection.RemoveRange(entitiesToRemove);
            if (saveChanges)
                await SaveChangesAsync();
        }

        internal void SaveDBChanges()
        {
            SaveChanges();
        }

        internal async Task SaveDBChangesAsync(CancellationToken cancellationToken)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }
}
