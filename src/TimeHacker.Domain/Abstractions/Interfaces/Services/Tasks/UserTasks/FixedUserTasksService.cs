using Helpers.Domain.Abstractions.Interfaces.IGenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks.UserTasks
{
    public interface IFixedUserTasksServiceCommand : IFixedTasksServiceCommand
    {
    }
    public interface IFixedUserTasksServiceQuery : IFixedTasksServiceQuery
    {
    }

    public class FixedUserTasksService : ServiceBase<IFixedUserTasksServiceCommand, IFixedUserTasksServiceQuery, FixedTask>
    {
        public FixedUserTasksService(IFixedUserTasksServiceCommand commands, IFixedUserTasksServiceQuery queries) : base(commands, queries) { }
    }
}
