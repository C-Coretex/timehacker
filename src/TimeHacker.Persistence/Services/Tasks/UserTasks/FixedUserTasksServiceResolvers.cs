using Helpers.DB.Abstractions.Classes.GenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Helpers;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks.UserTasks;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Persistence.Context;

namespace TimeHacker.Persistence.Services.Tasks.UserTasks
{
    public class FixedUserTasksServiceCommand : FixedTasksServiceCommand, IFixedUserTasksServiceCommand
    {
        public FixedUserTasksServiceCommand(TimeHackerDBContext dbContext) : base(dbContext) { }
    }

    public class FixedUserTasksServiceQuery : FixedTasksServiceQuery, IFixedUserTasksServiceQuery
    {
        private readonly IUserAccessor _userAccessor;
        public FixedUserTasksServiceQuery(TimeHackerDBContext dbContext, IUserAccessor userAccessor) : base(dbContext)
        {
            _userAccessor = userAccessor;
        }

        public override IQueryable<FixedTask> GetAll()
        {
            return base.GetAll().Where(x => x.UserId == _userAccessor.UserId);
        }

        public override FixedTask? GetById(int id)
        {
            var task = base.GetById(id);
            if (task?.UserId == _userAccessor.UserId)
                return task;

            return null;
        }

        public override async Task<FixedTask?> GetByIdAsync(int id)
        {
            var task = await base.GetByIdAsync(id);
            if (task?.UserId == _userAccessor.UserId)
                return task;

            return null;
        }
    }
}
