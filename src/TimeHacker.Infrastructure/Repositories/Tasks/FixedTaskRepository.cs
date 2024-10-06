using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tasks
{
    public class FixedTaskRepository : TaskRepository<FixedTask, Guid>, IFixedTaskRepository
    {
        public FixedTaskRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.FixedTask)
        { }
    }
}
