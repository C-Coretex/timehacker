using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public record InputWeekRepeatingEntityModel : InputRepeatingEntityModelBase
{
    [Required]
    public required IEnumerable<DayOfWeekEnum> RepeatsOn { get; set; }
    public override RepeatingEntityTypeEnum EntityType => RepeatingEntityTypeEnum.WeekRepeatingEntity;

    public override IRepeatingEntityType CreateEntity()
    {
        return new WeekRepeatingEntity(RepeatsOn);
    }
}
