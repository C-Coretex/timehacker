using Helpers.DB.Abstractions.Classes.GenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Tasks;
using TimeHacker.Persistence.Context;
namespace TimeHacker.Persistence.Services.Tasks
{ 
    public class DynamicTasksServiceCommand : ServiceCommandBase<TimeHackerDBContext, DynamicTask>, IDynamicTasksServiceCommand
    {
        public DynamicTasksServiceCommand(TimeHackerDBContext dbContext) : base(dbContext, dbContext.DynamicTasks) { }
    }

    public class DynamicTasksServiceQuery : ServiceQueryBase<DynamicTask>, IDynamicTasksServiceQuery
    {
        public DynamicTasksServiceQuery(TimeHackerDBContext dbContext) : base(dbContext.DynamicTasks) { }

        public IQueryable<DynamicTask> GetAllByUserId(string userId)
        {
            return GetAll().Where(x => x.UserId == userId);
        }
    }
}
