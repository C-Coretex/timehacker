using Helpers.Domain.Abstractions.Interfaces.IGenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Models.Tasks;

namespace TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks
{
    public interface IDynamicTasksServiceCommand : IServiceCommandBase<DynamicTask>
    {
    }
    public interface IDynamicTasksServiceQuery : IServiceQueryBase<DynamicTask>
    {
        public IQueryable<DynamicTask> GetAllByUserId(string userId);
    }

    public class DynamicTasksService : ServiceBase<IDynamicTasksServiceCommand, IDynamicTasksServiceQuery, DynamicTask>
    {
        public DynamicTasksService(IDynamicTasksServiceCommand commands, IDynamicTasksServiceQuery queries) : base(commands, queries) { }
    }
}
