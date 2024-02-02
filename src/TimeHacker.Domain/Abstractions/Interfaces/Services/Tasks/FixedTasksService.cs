﻿using Helpers.Domain.Abstractions.Interfaces.IGenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Models.Tasks;

namespace TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks
{
    public interface IFixedTasksServiceCommand : IServiceCommandBase<FixedTask>
    {
    }
    public interface IFixedTasksServiceQuery : IServiceQueryBase<FixedTask>
    {
    }

    public class FixedTasksService : ServiceBase<IFixedTasksServiceCommand, IFixedTasksServiceQuery, FixedTask>
    {
        public FixedTasksService(IFixedTasksServiceCommand commands, IFixedTasksServiceQuery queries) : base(commands, queries) { }
    }
}
