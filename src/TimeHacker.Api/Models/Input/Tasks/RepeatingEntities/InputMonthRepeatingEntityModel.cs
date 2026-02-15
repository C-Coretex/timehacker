using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public record InputMonthRepeatingEntityModel : InputRepeatingEntityModelBase
{
    [Required]
    public byte MonthDayToRepeat { get; set; }
    public override RepeatingEntityTypeEnum EntityType => RepeatingEntityTypeEnum.MonthRepeatingEntity;

    public override IRepeatingEntityType CreateEntity()
    {
        return new MonthRepeatingEntity(MonthDayToRepeat);
    }
}
