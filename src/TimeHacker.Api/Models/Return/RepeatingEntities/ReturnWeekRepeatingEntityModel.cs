using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnWeekRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public IEnumerable<Domain.Models.EntityModels.Enums.DayOfWeek> RepeatsOn { get; set; } = [];
    public override RepeatingEntityType EntityType => RepeatingEntityType.WeekRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var week = (WeekRepeatingEntity)dto.RepeatingData;
        return new ReturnWeekRepeatingEntityModel { RepeatsOn = week.RepeatsOn };
    }
}
