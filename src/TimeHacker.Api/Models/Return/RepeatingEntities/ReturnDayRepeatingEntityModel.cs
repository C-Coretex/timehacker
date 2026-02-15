using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnDayRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public int DaysCountToRepeat { get; set; }
    public override RepeatingEntityTypeEnum EntityType => RepeatingEntityTypeEnum.DayRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        var day = (DayRepeatingEntity)dto.RepeatingData;
        return new ReturnDayRepeatingEntityModel { DaysCountToRepeat = day.DaysCountToRepeat };
    }
}
