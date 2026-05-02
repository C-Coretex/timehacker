using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public record InputYearRepeatingEntityModel : InputRepeatingEntityModelBase
{
    [Required]
    public int YearDayToRepeat { get; set; }
    public override RepeatingEntityType EntityType => RepeatingEntityType.YearRepeatingEntity;

    public override IRepeatingEntityType CreateEntity()
    {
        return new YearRepeatingEntity(YearDayToRepeat);
    }
}
