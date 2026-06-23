using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Infrastructure.Repositories.Tasks;

internal class TaskRepository<TTask, TId>(TimeHackerDbContext dbContext, DbSet<TTask> dbSet, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : UserScopedRepositoryBase<TTask, TId>(dbContext, dbSet, userAccessor, timeProvider), ITaskRepository<TTask, TId>
    where TTask : class, IDbEntity<TId>, IUserScopedEntity
{
}
