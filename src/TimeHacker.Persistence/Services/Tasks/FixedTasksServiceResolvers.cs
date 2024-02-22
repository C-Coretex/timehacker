using Helpers.DB.Abstractions.Classes.GenericServices;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Persistence.Context;

namespace TimeHacker.Persistence.Services.Tasks
{
    public class FixedTasksServiceCommand : ServiceCommandBase<TimeHackerDBContext, FixedTask>, IFixedTasksServiceCommand
    {
        public FixedTasksServiceCommand(TimeHackerDBContext dbContext) : base(dbContext, dbContext.FixedTasks) { }
    }

    public class FixedTasksServiceQuery : ServiceQueryBase<FixedTask>, IFixedTasksServiceQuery
    {
        public FixedTasksServiceQuery(TimeHackerDBContext dbContext) : base(dbContext.FixedTasks) { }

        public IQueryable<FixedTask> GetAllByUserId(string userId)
        {
            return GetAll().Where(x => x.UserId == userId);
        }
    }
}
