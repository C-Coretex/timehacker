using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tasks
{
    internal class DynamicTaskRepository : TaskRepository<DynamicTask, Guid>, IDynamicTaskRepository
    {
        public DynamicTaskRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) : base(dbContext, dbContext.DynamicTask, userAccessor)
        {
        }
    }
}
