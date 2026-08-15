using TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

namespace TimeHacker.Api.Models.Input.Tasks;

public record InputScheduleEntityModel
{
    [Required]
    public Guid ParentEntityId { get; set; }
    [Required]
    public required InputRepeatingEntityModelBase RepeatingEntityType { get; set; }
    public EndsOnModel? EndsOnModel { get; set; }

    public ScheduleEntityCreateDto CreateDto(ScheduleEntityParentType parentType)
    {
        var repeatingEntityDto = new RepeatingEntityDto(RepeatingEntityType.EntityType, RepeatingEntityType.CreateEntity());
        return new ScheduleEntityCreateDto(parentType, ParentEntityId, repeatingEntityDto, EndsOnModel);
    }
}
