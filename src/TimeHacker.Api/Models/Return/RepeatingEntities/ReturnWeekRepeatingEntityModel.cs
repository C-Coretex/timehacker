using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnWeekRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public IEnumerable<DayOfWeekEnum> RepeatsOn { get; set; } = [];
    public override RepeatingEntityTypeEnum EntityType => RepeatingEntityTypeEnum.WeekRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        var week = (WeekRepeatingEntity)dto.RepeatingData;
        return new ReturnWeekRepeatingEntityModel { RepeatsOn = week.RepeatsOn };
    }
}
