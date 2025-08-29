using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduleEntityConfiguration : UserScopedEntityConfigurationBase<ScheduleEntity>
    {
        public override void Configure(EntityTypeBuilder<ScheduleEntity> builder)
        {
            ConfigureUserScoped(builder);

            builder.HasIndex(x => x.EndsOn);

            builder.Property(x => x.RepeatingEntity).IsRequired();

            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };
            builder.Property(x => x.RepeatingEntity).HasConversion(
                v => JsonSerializer.SerializeToUtf8Bytes(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<RepeatingEntityDto>(v, jsonSerializerOptions)!);
        }
    }
}
