using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnMonthRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public byte MonthDayToRepeat { get; set; }
    public override RepeatingEntityType EntityType => RepeatingEntityType.MonthRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var month = (MonthRepeatingEntity)dto.RepeatingData;
        return new ReturnMonthRepeatingEntityModel { MonthDayToRepeat = month.MonthDayToRepeat };
    }
}
