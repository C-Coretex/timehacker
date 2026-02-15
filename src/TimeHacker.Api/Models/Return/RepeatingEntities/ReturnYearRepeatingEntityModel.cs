using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnYearRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public int YearDayToRepeat { get; set; }
    public override RepeatingEntityTypeEnum EntityType => RepeatingEntityTypeEnum.YearRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        var year = (YearRepeatingEntity)dto.RepeatingData;
        return new ReturnYearRepeatingEntityModel { YearDayToRepeat = year.YearDayToRepeat };
    }
}
