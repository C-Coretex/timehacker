using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Helpers.Db.Abstractions.BaseClasses
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
                optionsBuilder.UseNpgsql(_connectionString);
        }

        internal T AddEntity<T>(DbSet<T> entityCollection, T entity) where T : class, IDbEntity
        {
            return entityCollection.Add(entity).Entity;
        }
        internal async Task<T> AddEntityAndSaveAsync<T>(DbSet<T> entityCollection, T entity, CancellationToken cancellationToken = default) where T : class, IDbEntity
        {
            var newEntity = entityCollection.Add(entity).Entity;
            await base.SaveChangesAsync(cancellationToken);

            return newEntity;
        }
        internal void AddEntities<T>(DbSet<T> entityCollection, IEnumerable<T> entities) where T : class, IDbEntity
        {
            entityCollection.AddRange(entities);
        }
        internal async Task AddEntitiesAndSaveAsync<T>(DbSet<T> entityCollection, IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IDbEntity
        {
            entityCollection.AddRange(entities);
            await SaveChangesAsync(cancellationToken);
        }

        internal T UpdateEntity<T>(DbSet<T> entityCollection, T entity) where T : class, IDbEntity
        {
            return entityCollection.Update(entity).Entity;
        }
        internal async Task<T> UpdateEntityAndSaveAsync<T>(DbSet<T> entityCollection, T entity, CancellationToken cancellationToken = default) where T : class, IDbEntity
        {
            var newEntity = entityCollection.Update(entity).Entity;
            await SaveChangesAsync(cancellationToken);

            return newEntity;
        }
        internal void UpdateEntities<T>(DbSet<T> entityCollection, IEnumerable<T> entities) where T : class, IDbEntity
        {
            entityCollection.UpdateRange(entities);
        }
        internal async Task UpdateEntitiesAndSaveAsync<T>(DbSet<T> entityCollection, IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IDbEntity
        {
            entityCollection.UpdateRange(entities);
            await SaveChangesAsync(cancellationToken);
        }

        internal void RemoveEntity<T>(DbSet<T> entityCollection, T model) where T : class, IDbEntity
        {
            entityCollection.Remove(model);
        }
        internal async Task RemoveEntityAndSaveAsync<T>(DbSet<T> entityCollection, T model, CancellationToken cancellationToken = default) where T : class, IDbEntity, new()
        {
            entityCollection.Remove(model);
            await SaveChangesAsync(cancellationToken);
        }
        internal void RemoveEntities<T>(DbSet<T> entityCollection, IEnumerable<T> models) where T : class, IDbEntity
        {
            entityCollection.RemoveRange(models);
        }
        internal async Task RemoveEntitiesAndSaveAsync<T>(DbSet<T> entityCollection, IEnumerable<T> models, CancellationToken cancellationToken = default) where T : class, IDbEntity, new()
        {
            entityCollection.RemoveRange(models);
            await SaveChangesAsync(cancellationToken);
        }

        internal void RemoveEntity<T, TId>(DbSet<T> entityCollection, TId id) where T : class, IDbEntity<TId>, new()
        {
            entityCollection.Remove(new T() { Id = id });
        }
        internal async Task RemoveEntityAndSaveAsync<T, TId>(DbSet<T> entityCollection, TId id, CancellationToken cancellationToken = default) where T : class, IDbEntity<TId>, new()
        {
            entityCollection.Remove(new T() { Id = id });
            await SaveChangesAsync(cancellationToken);
        }
        internal void RemoveEntities<T, TId>(DbSet<T> entityCollection, IEnumerable<TId> ids) where T : class, IDbEntity<TId>, new()
        {
            entityCollection.RemoveRange(ids.Select(id => new T() { Id = id }));
        }
        internal async Task RemoveEntitiesAndSaveAsync<T, TId>(DbSet<T> entityCollection, IEnumerable<TId> ids, CancellationToken cancellationToken = default) where T : class, IDbEntity<TId>, new()
        {
            entityCollection.RemoveRange(ids.Select(id => new T() { Id = id }));
            await SaveChangesAsync(cancellationToken);
        }

        internal void RemoveEntities<T>(DbSet<T> entityCollection, Func<T, bool> selector) where T : class, IDbEntity
        {
            var entitiesToRemove = entityCollection.Where(selector);
            entityCollection.RemoveRange(entitiesToRemove);
        }
        internal async Task RemoveEntitiesAndSaveAsync<T>(DbSet<T> entityCollection, Func<T, bool> selector, CancellationToken cancellationToken = default) where T : class, IDbEntity
        {
            var entitiesToRemove = entityCollection.Where(selector);
            entityCollection.RemoveRange(entitiesToRemove);
            await SaveChangesAsync(cancellationToken);
        }
    }
}
