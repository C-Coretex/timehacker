﻿using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices;
using TimeHacker.Domain.Models.ReturnModels;

namespace TimeHacker.Domain.Services.Services
{
    public class ScheduleEntityService(IScheduleEntityRepository scheduleEntityRepository) : IScheduleEntityService
    {
        public IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from)
        {
            var query = scheduleEntityRepository.GetAll().Include(x => x.FixedTask)
                .Where(x => x.EndsOn == null || x.EndsOn >= from);

            return query.Select(scheduleEntity => new ScheduleEntityReturn()
            {
                Id = scheduleEntity.Id,
                UserId = scheduleEntity.UserId,
                RepeatingEntity = scheduleEntity.RepeatingEntity,
                CreatedTimestamp = scheduleEntity.CreatedTimestamp,
                LastEntityCreated = scheduleEntity.LastEntityCreated,
                EndsOn = scheduleEntity.EndsOn,
                ScheduledTasks = scheduleEntity.ScheduledTasks,
                ScheduledCategories = scheduleEntity.ScheduledCategories,
                FixedTask = scheduleEntity.FixedTask,
                Category = scheduleEntity.Category
            });
        }

        public async Task UpdateLastEntityCreated(Guid id, DateOnly entityCreated)
        {
            var scheduleEntity = await scheduleEntityRepository.GetByIdAsync(id);
            if (scheduleEntity == null)
                return;

            var scheduleEntityReturn = ScheduleEntityReturn.Create(scheduleEntity);
            if (!scheduleEntityReturn.IsEntityDateCorrect(entityCreated))
                throw new DataIsNotCorrectException("Created entity timestamp is not correct", nameof(entityCreated));

            if (scheduleEntity.LastEntityCreated != null && scheduleEntity.LastEntityCreated >= entityCreated)
                return;

            scheduleEntity.LastEntityCreated = entityCreated;
            await scheduleEntityRepository.UpdateAndSaveAsync(scheduleEntity);
        }
    }
}
