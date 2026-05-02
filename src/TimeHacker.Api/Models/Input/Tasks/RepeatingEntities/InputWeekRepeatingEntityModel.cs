using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public record InputWeekRepeatingEntityModel : InputRepeatingEntityModelBase
{
    [Required]
    public required IEnumerable<Domain.Models.EntityModels.Enums.DayOfWeek> RepeatsOn { get; set; }
    public override RepeatingEntityType EntityType => RepeatingEntityType.WeekRepeatingEntity;

    public override IRepeatingEntityType CreateEntity()
    {
        return new WeekRepeatingEntity(RepeatsOn);
    }
}
