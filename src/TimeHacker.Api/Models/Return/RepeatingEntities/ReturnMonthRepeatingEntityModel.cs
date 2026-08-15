namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnMonthRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public byte MonthDayToRepeat { get; set; }
    public override RepeatingEntityType EntityType => RepeatingEntityType.MonthRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var month = (MonthRepeatingEntity)dto.RepeatingData;
        return new ReturnMonthRepeatingEntityModel { MonthDayToRepeat = month.MonthDayToRepeat };
    }
}
