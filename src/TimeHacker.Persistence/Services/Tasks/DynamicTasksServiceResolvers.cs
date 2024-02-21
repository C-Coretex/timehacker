using Helpers.DB.Abstractions.Classes.GenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Helpers;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Persistence.Context;
namespace TimeHacker.Persistence.Services.Tasks
{
    public class DynamicTasksServiceCommand : ServiceCommandBase<TimeHackerDBContext, DynamicTask>, IDynamicTasksServiceCommand
    {
        public DynamicTasksServiceCommand(TimeHackerDBContext dbContext) : base(dbContext, dbContext.DynamicTasks) { }
    }

    public class DynamicTasksServiceQuery : ServiceQueryBase<DynamicTask>, IDynamicTasksServiceQuery
    {
        private readonly IUserAccessor _userAccessor;
        public DynamicTasksServiceQuery(TimeHackerDBContext dbContext, IUserAccessor userAccessor) : base(dbContext.DynamicTasks) 
        {
            _userAccessor = userAccessor;
        }

        public override IQueryable<DynamicTask> GetAll()
        {
            return base.GetAll().Where(x => x.UserId == _userAccessor.UserId);
        }

        public override DynamicTask? GetById(int id)
        {
            var task = base.GetById(id);
            if(task?.UserId == _userAccessor.UserId)
                return task;

            return null;
        }

        public override async Task<DynamicTask?> GetByIdAsync(int id)
        {
            var task = await base.GetByIdAsync(id);
            if (task?.UserId == _userAccessor.UserId)
                return task;

            return null;
        }

        public IQueryable<DynamicTask> GetAllByUserId(string userId)
        {
            return GetAll().Where(x => x.UserId == userId);
        }
    }
}
