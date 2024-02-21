using Helpers.DB.Abstractions.Classes.GenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Helpers;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks.UserTasks;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Persistence.Context;

namespace TimeHacker.Persistence.Services.Tasks.UserTasks
{
    public class DynamicUserTasksServiceCommand : DynamicTasksServiceCommand, IDynamicUserTasksServiceCommand
    {
        public DynamicUserTasksServiceCommand(TimeHackerDBContext dbContext) : base(dbContext) { }
    }

    public class DynamicUserTasksServiceQuery : DynamicTasksServiceQuery, IDynamicUserTasksServiceQuery
    {
        private readonly IUserAccessor _userAccessor;
        public DynamicUserTasksServiceQuery(TimeHackerDBContext dbContext, IUserAccessor userAccessor) : base(dbContext)
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
            if (task?.UserId == _userAccessor.UserId)
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
    }
}
