﻿using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Helpers.DB.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.ScheduleSnapshots
{
    public class ScheduledCategoryRepository : RepositoryBase<TimeHackerDbContext, ScheduledCategory, Guid>, IScheduledCategoryRepository
    {
        public ScheduledCategoryRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.ScheduledCategory)
        { }
    }
}