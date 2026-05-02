using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public abstract record InputRepeatingEntityModelBase
{
    public abstract RepeatingEntityType EntityType { get; }
    public abstract IRepeatingEntityType CreateEntity();
}
