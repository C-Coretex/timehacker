using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Domain.DTOs.RepeatingEntity
{
    public record RepeatingEntityDto(
        RepeatingEntityTypeEnum EntityType,
        IRepeatingEntityType RepeatingData
        )
    {
    }
}
