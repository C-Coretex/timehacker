using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.BusinessLogicExceptions;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Categories;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.ReturnModels;
using TimeHacker.Domain.IncludeExpansionDelegates;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduleEntityService(
        IScheduleEntityRepository scheduleEntityRepository,
        IFixedTaskService fixedTaskService,
        ICategoryService categoryService,
        UserAccessorBase userAccessorBase)
        : IScheduleEntityService
    {
        public IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from)
        {
            var query = scheduleEntityRepository.GetAll(IncludeExpansionScheduleEntity.IncludeFixedTask)
                .Where(x => x.UserId == userAccessorBase.UserId && (x.EndsOn == null || x.EndsOn >= from));

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

            //TODO: will be removed after repository level filtrations, it will just be null and another exception will be thrown
            if (scheduleEntity.UserId != userAccessorBase.UserId)
                throw new Exception("User can edit only his own ScheduleEntity.");

            var scheduleEntityReturn = ScheduleEntityReturn.Create(scheduleEntity);
            if (!scheduleEntityReturn.IsEntityDateCorrect(entityCreated))
                throw new DataIsNotCorrectException("Created entity timestamp is not correct", nameof(entityCreated));

            if (scheduleEntity.LastEntityCreated != null && scheduleEntity.LastEntityCreated >= entityCreated)
                return;

            scheduleEntity.LastEntityCreated = entityCreated;
            await scheduleEntityRepository.UpdateAndSaveAsync(scheduleEntity);
        }

        public Task<ScheduleEntity> Save(InputScheduleEntityModel inputScheduleEntity)
        {
            var scheduleEntity = inputScheduleEntity.GetScheduleEntity();
            scheduleEntity.UserId = userAccessorBase.UserId!;

            return inputScheduleEntity.ScheduleEntityParentEnum switch
            {
                ScheduleEntityParentEnum.FixedTask => fixedTaskService.UpdateScheduleEntityAsync(scheduleEntity, inputScheduleEntity.ParentEntityId),
                ScheduleEntityParentEnum.Category => categoryService.UpdateScheduleEntityAsync(scheduleEntity, inputScheduleEntity.ParentEntityId),
                _ => throw new NotProvidedException(nameof(inputScheduleEntity.ScheduleEntityParentEnum)),
            };
        }
    }
}
