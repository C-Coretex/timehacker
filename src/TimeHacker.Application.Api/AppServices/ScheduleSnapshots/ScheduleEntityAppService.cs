using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Application.Api.AppServices.ScheduleSnapshots
{
    public class ScheduleEntityAppService(
        IScheduleEntityRepository scheduleEntityRepository,
        IFixedTaskRepository fixedTaskRepository,
        ICategoryRepository categoryRepository) : IScheduleEntityAppService
    {
        public async Task<ScheduleEntity> Save(InputScheduleEntityModel inputScheduleEntity)
        {
            switch (inputScheduleEntity.ScheduleEntityParentEnum)
            {
                case ScheduleEntityParentEnum.FixedTask:
                    if(!await fixedTaskRepository.ExistsAsync(inputScheduleEntity.ParentEntityId))
                        throw new NotFoundException(nameof(ScheduleEntityParentEnum.FixedTask), inputScheduleEntity.ParentEntityId.ToString());
                    break;
                case ScheduleEntityParentEnum.Category:
                    if (!await categoryRepository.ExistsAsync(inputScheduleEntity.ParentEntityId))
                        throw new NotFoundException(nameof(ScheduleEntityParentEnum.Category), inputScheduleEntity.ParentEntityId.ToString());
                    break;
                default:
                    throw new NotProvidedException(nameof(inputScheduleEntity.ScheduleEntityParentEnum));
            }

            var scheduleEntity = inputScheduleEntity.GetScheduleEntity();
            scheduleEntity = await scheduleEntityRepository.AddAndSaveAsync(scheduleEntity);

            switch (inputScheduleEntity.ScheduleEntityParentEnum)
            {
                case ScheduleEntityParentEnum.FixedTask:
                    await fixedTaskRepository.UpdateProperty(
                        x => x.Id == inputScheduleEntity.ParentEntityId,
                        x => x.ScheduleEntityId,
                        scheduleEntity.Id);
                    break;
                case ScheduleEntityParentEnum.Category:
                    await categoryRepository.UpdateProperty(
                        x => x.Id == inputScheduleEntity.ParentEntityId,
                        x => x.ScheduleEntityId,
                        scheduleEntity.Id);
                    break;
                default:
                    throw new NotProvidedException(nameof(inputScheduleEntity.ScheduleEntityParentEnum));
            }
            

            return scheduleEntity;
        }
    }
}
