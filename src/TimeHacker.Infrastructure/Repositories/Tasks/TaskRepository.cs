using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Infrastructure.Repositories.Tasks
{
    public class TaskRepository<TTask, TId> : RepositoryBase<TimeHackerDbContext, TTask, TId>, ITaskRepository<TTask, TId>
        where TTask : class, IDbModel<TId>, new()
    {
        public TaskRepository(TimeHackerDbContext dbContext, DbSet<TTask> dbSet) : base(dbContext, dbSet)
        {
        }
    }
}
