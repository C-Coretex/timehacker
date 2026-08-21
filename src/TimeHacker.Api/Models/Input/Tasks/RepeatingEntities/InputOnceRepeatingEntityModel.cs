namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

public record InputOnceRepeatingEntityModel : InputRepeatingEntityModelBase
{
    [Required]
    [MinLength(1)]
    public required IEnumerable<DateOnly> Dates { get; set; }
    public override RepeatingEntityType EntityType => RepeatingEntityType.OnceRepeatingEntity;

    public override IRepeatingEntityType CreateEntity()
    {
        return new OnceRepeatingEntity(Dates);
    }
}
