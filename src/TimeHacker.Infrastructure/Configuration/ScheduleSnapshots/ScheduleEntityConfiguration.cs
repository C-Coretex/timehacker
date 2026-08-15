using TimeHacker.Domain.DTOs.RepeatingEntity;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots;

public class ScheduleEntityConfiguration : UserScopedEntityConfigurationBase<ScheduleEntity>
{
    public override void Configure(EntityTypeBuilder<ScheduleEntity> builder)
    {
        ConfigureUserScoped(builder);

        builder.HasIndex(x => x.EndsOn);

        builder.Property(x => x.RepeatingEntity).IsRequired();

        // RepeatingEntity is stored as BSON. The DefaultJsonTypeInfoResolver enables the polymorphic
        // [JsonDerivedType] discriminators on IRepeatingEntityType, so the concrete recurrence subtype
        // (day/week/month/year) round-trips correctly.
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };
        builder.Property(x => x.RepeatingEntity).HasConversion(
            v => JsonSerializer.SerializeToUtf8Bytes(v, jsonSerializerOptions),
            v => JsonSerializer.Deserialize<RepeatingEntityDto>(v, jsonSerializerOptions)!);
    }
}
