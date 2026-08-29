using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Domain.Helpers.ScheduleSnapshots;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Application.Api.AppServices.ScheduleSnapshots;

public class ScheduleEntityAppService(
    IScheduleEntityRepository scheduleEntityRepository,
    IFixedTaskRepository fixedTaskRepository,
    ICategoryRepository categoryRepository,
    TimeProvider timeProvider) : IScheduleEntityAppService
{
    /// <summary>
    /// Creates a recurrence ScheduleEntity and attaches it to its polymorphic parent
    /// (a FixedTask or a Category). Done in two phases around the persist: first read the chosen parent —
    /// both to validate it exists and to take the day it already occupies as the recurrence's anchor —
    /// then, after the entity has an Id, stamp that Id onto the parent.
    /// </summary>
    public async Task<ScheduleEntityDto> Save(ScheduleEntityCreateDto scheduleEntityCreateDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(scheduleEntityCreateDto);
        NotProvidedException.ThrowIfNull(scheduleEntityCreateDto.RepeatingEntityModel, propertyName: nameof(scheduleEntityCreateDto));

        // Phase 1: read the parent and take its own day as the anchor.
        DateOnly anchorDate;
        switch (scheduleEntityCreateDto.ScheduleEntityParentEnum)
        {
            case ScheduleEntityParentType.FixedTask:
                var fixedTask = await fixedTaskRepository.GetByIdAsync(scheduleEntityCreateDto.ParentEntityId, cancellationToken: cancellationToken);
                NotFoundException.ThrowIfNull(fixedTask, nameof(ScheduleEntityParentType.FixedTask), scheduleEntityCreateDto.ParentEntityId.ToString());
                anchorDate = DateOnly.FromDateTime(fixedTask.StartTimestamp);
                break;
            case ScheduleEntityParentType.Category:
                var category = await categoryRepository.GetByIdAsync(scheduleEntityCreateDto.ParentEntityId, cancellationToken: cancellationToken);
                NotFoundException.ThrowIfNull(category, nameof(ScheduleEntityParentType.Category), scheduleEntityCreateDto.ParentEntityId.ToString());
                anchorDate = category.Date;
                break;
            default:
                throw new NotProvidedException(nameof(scheduleEntityCreateDto));
        }

        var scheduleEntity = ScheduleEntityHelper.GetScheduleEntity(scheduleEntityCreateDto.RepeatingEntityModel, scheduleEntityCreateDto.EndsOnModel, anchorDate, timeProvider);
        scheduleEntity = await scheduleEntityRepository.AddAndSaveAsync(scheduleEntity, cancellationToken);

        // Phase 2: wire the now-persisted entity's Id back onto the parent.
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
