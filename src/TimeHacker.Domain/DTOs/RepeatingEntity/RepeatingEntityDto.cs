using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Domain.DTOs.RepeatingEntity;

public record RepeatingEntityDto(
    RepeatingEntityType EntityType,
    IRepeatingEntityType RepeatingData
    )
{
}
