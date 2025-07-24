using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Infrastructure.Repositories.Tasks
{
    public class TaskRepository<TTask, TId> : UserScopedRepositoryBase<TTask, TId>, ITaskRepository<TTask, TId>
        where TTask : class, IDbEntity<TId>, IUserScopedEntity
    {
        public TaskRepository(TimeHackerDbContext dbContext, DbSet<TTask> dbSet, UserAccessorBase userAccessor) : base(dbContext, dbSet, userAccessor)
        {
        }
    }
}
