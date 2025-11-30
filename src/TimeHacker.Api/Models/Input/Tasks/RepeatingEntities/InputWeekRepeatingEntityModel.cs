using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public record InputWeekRepeatingEntityModel : IInputRepeatingEntityType
{
    [Required]
    public required IEnumerable<DayOfWeekEnum> RepeatsOn { get; set; }
    public RepeatingEntityTypeEnum EntityType { get; init; } = RepeatingEntityTypeEnum.WeekRepeatingEntity;

    public IRepeatingEntityType CreateEntity()
    {
        return new WeekRepeatingEntity(RepeatsOn);
    }
}
