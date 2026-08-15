namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnDayRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public int DaysCountToRepeat { get; set; }
    public override RepeatingEntityType EntityType => RepeatingEntityType.DayRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var day = (DayRepeatingEntity)dto.RepeatingData;
        return new ReturnDayRepeatingEntityModel { DaysCountToRepeat = day.DaysCountToRepeat };
    }
}
