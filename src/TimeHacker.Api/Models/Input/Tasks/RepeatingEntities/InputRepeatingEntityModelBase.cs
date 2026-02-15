using TimeHacker.Api.Models.Return.RepeatingEntities;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public abstract record InputRepeatingEntityModelBase
{
    public abstract RepeatingEntityTypeEnum EntityType { get; }
    public abstract IRepeatingEntityType CreateEntity();
}
