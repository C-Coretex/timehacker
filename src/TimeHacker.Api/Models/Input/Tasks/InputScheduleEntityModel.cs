using TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;
using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Api.Models.Input.Tasks;

public record InputScheduleEntityModel
{
    [Required]
    public Guid ParentEntityId { get; set; }
    [Required]
    public required IInputRepeatingEntityType RepeatingEntityType { get; set; }
    public EndsOnModel? EndsOnModel { get; set; } = null;

    public ScheduleEntityCreateDto CreateDto()
    {
        var repeatingEntityDto = new RepeatingEntityDto(RepeatingEntityType.EntityType, RepeatingEntityType.CreateEntity());
        return new ScheduleEntityCreateDto(ScheduleEntityParentEnum.FixedTask, ParentEntityId, repeatingEntityDto, EndsOnModel);
    }
}
