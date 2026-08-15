namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnYearRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public int YearDayToRepeat { get; set; }
    public override RepeatingEntityType EntityType => RepeatingEntityType.YearRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var year = (YearRepeatingEntity)dto.RepeatingData;
        return new ReturnYearRepeatingEntityModel { YearDayToRepeat = year.YearDayToRepeat };
    }
}
