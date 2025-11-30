using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public interface IInputRepeatingEntityType
{
    RepeatingEntityTypeEnum EntityType { get; init; }
    IRepeatingEntityType CreateEntity();
}
