using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.Helpers.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Application.Api.AppServices.ScheduleSnapshots;

public class ScheduleEntityAppService(
    IScheduleEntityRepository scheduleEntityRepository,
    IFixedTaskRepository fixedTaskRepository,
    ICategoryRepository categoryRepository) : IScheduleEntityAppService
{
    public async Task<ScheduleEntityDto> Save(ScheduleEntityCreateDto scheduleEntityCreateDto)
    {
        switch (scheduleEntityCreateDto.ScheduleEntityParentEnum)
        {
            case ScheduleEntityParentEnum.FixedTask:
                if(!await fixedTaskRepository.ExistsAsync(scheduleEntityCreateDto.ParentEntityId))
                    throw new NotFoundException(nameof(ScheduleEntityParentEnum.FixedTask), scheduleEntityCreateDto.ParentEntityId.ToString());
                break;
            case ScheduleEntityParentEnum.Category:
                if (!await categoryRepository.ExistsAsync(scheduleEntityCreateDto.ParentEntityId))
                    throw new NotFoundException(nameof(ScheduleEntityParentEnum.Category), scheduleEntityCreateDto.ParentEntityId.ToString());
                break;
            default:
                throw new NotProvidedException(nameof(scheduleEntityCreateDto));
        }

        var scheduleEntity = ScheduleEntityHelper.GetScheduleEntity(scheduleEntityCreateDto.RepeatingEntityModel, scheduleEntityCreateDto.EndsOnModel);
        scheduleEntity = await scheduleEntityRepository.AddAndSaveAsync(scheduleEntity);

        switch (scheduleEntityCreateDto.ScheduleEntityParentEnum)
        {
            case ScheduleEntityParentEnum.FixedTask:
                await fixedTaskRepository.UpdateProperty(
                    x => x.Id == scheduleEntityCreateDto.ParentEntityId,
                    x => x.ScheduleEntityId,
                    scheduleEntity.Id);
                break;
            case ScheduleEntityParentEnum.Category:
                await categoryRepository.UpdateProperty(
                    x => x.Id == scheduleEntityCreateDto.ParentEntityId,
                    x => x.ScheduleEntityId,
                    scheduleEntity.Id);
                break;
            default:
                throw new NotProvidedException(nameof(scheduleEntityCreateDto));
        }
        
        return ScheduleEntityDto.Create(scheduleEntity);
    }
}
