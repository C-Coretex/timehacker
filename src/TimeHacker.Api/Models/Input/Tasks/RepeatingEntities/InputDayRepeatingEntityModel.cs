using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public record InputDayRepeatingEntityModel : InputRepeatingEntityModelBase
{
    [Required]
    public int DaysCountToRepeat { get; set; }
    public override RepeatingEntityType EntityType => RepeatingEntityType.DayRepeatingEntity;

    public override IRepeatingEntityType CreateEntity()
    {
        return new DayRepeatingEntity(DaysCountToRepeat);
    }
}
