using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnMonthRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public byte MonthDayToRepeat { get; set; }
    public override RepeatingEntityTypeEnum EntityType => RepeatingEntityTypeEnum.MonthRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        var month = (MonthRepeatingEntity)dto.RepeatingData;
        return new ReturnMonthRepeatingEntityModel { MonthDayToRepeat = month.MonthDayToRepeat };
    }
}
