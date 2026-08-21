namespace TimeHacker.Api.Models.Return.RepeatingEntities;

public record ReturnOnceRepeatingEntityModel : ReturnRepeatingEntityModelBase
{
    public IEnumerable<DateOnly> Dates { get; set; } = [];
    public override RepeatingEntityType EntityType => RepeatingEntityType.OnceRepeatingEntity;

    public override ReturnRepeatingEntityModelBase CreateFromEntity(RepeatingEntityDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var once = (OnceRepeatingEntity)dto.RepeatingData;
        return new ReturnOnceRepeatingEntityModel { Dates = once.Dates };
    }
}
