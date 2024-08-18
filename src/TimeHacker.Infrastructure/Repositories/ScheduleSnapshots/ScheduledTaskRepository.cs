﻿using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.DB.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduledTaskRepository: RepositoryBase<TimeHackerDbContext, ScheduledTask, Guid>, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduledTask)
        { }
    }
}