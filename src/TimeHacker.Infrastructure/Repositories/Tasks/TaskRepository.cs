using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Infrastructure.Repositories.Tasks
{
    public class TaskRepository<TTask, TId> : RepositoryBase<TimeHackerDbContext, TTask, TId>, ITaskRepository<TTask, TId>
        where TTask : class, IDbEntity<TId>, new()
    {
        public TaskRepository(TimeHackerDbContext dbContext, DbSet<TTask> dbSet) : base(dbContext, dbSet)
        {
        }
    }
}
