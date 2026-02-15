using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Return.RepeatingEntities;

[JsonDerivedType(typeof(ReturnDayRepeatingEntityModel), "day")]
[JsonDerivedType(typeof(ReturnWeekRepeatingEntityModel), "week")]
[JsonDerivedType(typeof(ReturnMonthRepeatingEntityModel), "month")]
[JsonDerivedType(typeof(ReturnYearRepeatingEntityModel), "year")]
public abstract record ReturnRepeatingEntityModelBase
{
    public abstract RepeatingEntityTypeEnum EntityType { get; }
    public abstract ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto);

    public static ReturnRepeatingEntityModelBase Create(RepeatingEntityDto dto) => dto.RepeatingData switch
    {
        DayRepeatingEntity => new ReturnDayRepeatingEntityModel().CreateFromEntity(dto),
        WeekRepeatingEntity => new ReturnWeekRepeatingEntityModel().CreateFromEntity(dto),
        MonthRepeatingEntity => new ReturnMonthRepeatingEntityModel().CreateFromEntity(dto),
        YearRepeatingEntity => new ReturnYearRepeatingEntityModel().CreateFromEntity(dto),
        _ => throw new ArgumentOutOfRangeException(nameof(dto.RepeatingData), $"Unknown repeating entity type: {dto.RepeatingData.GetType().Name}")
    };
}
