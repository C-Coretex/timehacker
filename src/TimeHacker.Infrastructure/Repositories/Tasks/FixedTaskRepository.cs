using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tasks
{
    public class FixedTaskRepository : TaskRepository<FixedTask, Guid>, IFixedTaskRepository
    {
        public FixedTaskRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.FixedTask)
        { }
    }
}
