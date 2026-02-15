using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public record InputYearRepeatingEntityModel : InputRepeatingEntityModelBase
{
    [Required]
    public int YearDayToRepeat { get; set; }
    public override RepeatingEntityTypeEnum EntityType => RepeatingEntityTypeEnum.YearRepeatingEntity;

    public override IRepeatingEntityType CreateEntity()
    {
        return new YearRepeatingEntity(YearDayToRepeat);
    }
}
