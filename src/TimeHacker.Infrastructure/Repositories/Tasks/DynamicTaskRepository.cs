﻿using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;

namespace TimeHacker.Infrastructure.Repositories.Tasks
{
    internal class DynamicTaskRepository : TaskRepository<DynamicTask, int>, IDynamicTaskRepository
    {
        public DynamicTaskRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.DynamicTask)
        {
        }
    }
}
