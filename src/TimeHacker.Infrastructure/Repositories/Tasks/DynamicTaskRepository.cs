using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tasks
{
    internal class DynamicTaskRepository : TaskRepository<DynamicTask, Guid>, IDynamicTaskRepository
    {
        public DynamicTaskRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.DynamicTask)
        {
        }
    }
}
