namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnWeekRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public IEnumerable<Domain.Models.EntityModels.Enums.DayOfWeek> RepeatsOn { get; set; } = [];
    public override RepeatingEntityType EntityType => RepeatingEntityType.WeekRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var week = (WeekRepeatingEntity)dto.RepeatingData;
        return new ReturnWeekRepeatingEntityModel { RepeatsOn = week.RepeatsOn };
    }
}
