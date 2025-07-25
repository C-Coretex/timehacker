using TimeHacker.Application.Api.Contracts.IServices.Categories;
using TimeHacker.Application.Api.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IServices.Tasks;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Models.ReturnModels;
using TimeHacker.Domain.Services.IncludeExpansionDelegates;

namespace TimeHacker.Application.Api.Services.ScheduleSnapshots
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

        public Task<ScheduleEntity> Save(InputScheduleEntityModel inputScheduleEntity)
        {
            var scheduleEntity = inputScheduleEntity.GetScheduleEntity();
            scheduleEntity.UserId = userAccessorBase.GetUserIdOrThrowUnauthorized()!;

            return inputScheduleEntity.ScheduleEntityParentEnum switch
            {
                ScheduleEntityParentEnum.FixedTask => fixedTaskService.UpdateScheduleEntityAsync(scheduleEntity, inputScheduleEntity.ParentEntityId),
                ScheduleEntityParentEnum.Category => categoryService.UpdateScheduleEntityAsync(scheduleEntity, inputScheduleEntity.ParentEntityId),
                _ => throw new NotProvidedException(nameof(inputScheduleEntity.ScheduleEntityParentEnum)),
            };
        }
    }
}
