using Helpers.Domain.Abstractions.Interfaces.IGenericServices;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks.UserTasks
{
    public interface IDynamicUserTasksServiceCommand : IDynamicTasksServiceCommand
    {
    }
    public interface IDynamicUserTasksServiceQuery : IDynamicTasksServiceQuery
    {
    }

    public class DynamicUserTasksService : ServiceBase<IDynamicUserTasksServiceCommand, IDynamicUserTasksServiceQuery, DynamicTask>
    {
        public DynamicUserTasksService(IDynamicUserTasksServiceCommand commands, IDynamicUserTasksServiceQuery queries) : base(commands, queries) { }
    }
}
