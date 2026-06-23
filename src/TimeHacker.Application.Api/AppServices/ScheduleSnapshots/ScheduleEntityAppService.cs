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
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider) : IScheduleEntityAppService
{
    public async Task<ScheduleEntityDto> Save(ScheduleEntityCreateDto scheduleEntityCreateDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(scheduleEntityCreateDto);
        NotProvidedException.ThrowIfNull(scheduleEntityCreateDto.RepeatingEntityModel, propertyName: nameof(scheduleEntityCreateDto));

        switch (scheduleEntityCreateDto.ScheduleEntityParentEnum)
        {
            case ScheduleEntityParentType.FixedTask:
                if(!await fixedTaskRepository.ExistsAsync(scheduleEntityCreateDto.ParentEntityId, cancellationToken))
                    throw new NotFoundException(nameof(ScheduleEntityParentType.FixedTask), scheduleEntityCreateDto.ParentEntityId.ToString());
                break;
            case ScheduleEntityParentType.Category:
                if (!await categoryRepository.ExistsAsync(scheduleEntityCreateDto.ParentEntityId, cancellationToken))
                    throw new NotFoundException(nameof(ScheduleEntityParentType.Category), scheduleEntityCreateDto.ParentEntityId.ToString());
                break;
            default:
                throw new NotProvidedException(nameof(scheduleEntityCreateDto));
        }

        var scheduleEntity = ScheduleEntityHelper.GetScheduleEntity(scheduleEntityCreateDto.RepeatingEntityModel, scheduleEntityCreateDto.EndsOnModel, timeProvider);
        scheduleEntity = await scheduleEntityRepository.AddAndSaveAsync(scheduleEntity, cancellationToken);

        switch (scheduleEntityCreateDto.ScheduleEntityParentEnum)
        {
            case ScheduleEntityParentType.FixedTask:
                await fixedTaskRepository.UpdateProperty(
                    x => x.Id == scheduleEntityCreateDto.ParentEntityId,
                    x => x.ScheduleEntityId,
                    scheduleEntity.Id,
                    cancellationToken);
                break;
            case ScheduleEntityParentType.Category:
                await categoryRepository.UpdateProperty(
                    x => x.Id == scheduleEntityCreateDto.ParentEntityId,
                    x => x.ScheduleEntityId,
                    scheduleEntity.Id,
                    cancellationToken);
                break;
            default:
                throw new NotProvidedException(nameof(scheduleEntityCreateDto));
        }

        return ScheduleEntityDto.Create(scheduleEntity);
    }
}
