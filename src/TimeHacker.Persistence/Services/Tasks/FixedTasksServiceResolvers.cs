﻿using Helpers.DB.Abstractions.Classes.GenericServices;
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
    public class FixedTasksServiceCommand : ServiceCommandBase<TimeHackerDBContext, FixedTask>, IFixedTasksServiceCommand
    {
        public FixedTasksServiceCommand(TimeHackerDBContext dbContext) : base(dbContext, dbContext.FixedTasks) { }
    }

    public class FixedTasksServiceQuery : ServiceQueryBase<FixedTask>, IFixedTasksServiceQuery
    {
        public FixedTasksServiceQuery(TimeHackerDBContext dbContext) : base(dbContext.FixedTasks) { }
    }
}
