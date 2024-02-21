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
    public class FixedTasksServiceCommand : ServiceCommandBase<TimeHackerDBContext, FixedTask>, IFixedTasksServiceCommand
    {
        public FixedTasksServiceCommand(TimeHackerDBContext dbContext) : base(dbContext, dbContext.FixedTasks) { }
    }

    public class FixedTasksServiceQuery : ServiceQueryBase<FixedTask>, IFixedTasksServiceQuery
    {
        private readonly IUserAccessor _userAccessor;
        public FixedTasksServiceQuery(IUserAccessor userAccessor, TimeHackerDBContext dbContext) : base(dbContext.FixedTasks) 
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

        public IQueryable<FixedTask> GetAllByUserId(string userId)
        {
            return GetAll().Where(x => x.UserId == userId);
        }
    }
}
