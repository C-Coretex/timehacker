using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Models.EntityModels;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduleEntityConfiguration : UserScopedEntityConfigurationBase<ScheduleEntity>
    {
        public override void Configure(EntityTypeBuilder<ScheduleEntity> builder)
        {
            ConfigureUserScoped(builder);

            builder.HasIndex(x => x.EndsOn);

            builder.Property(x => x.RepeatingEntity).IsRequired();

            var jsonSerializerOptions = new JsonSerializerOptions();
            builder.Property(x => x.RepeatingEntity).HasConversion(
                v => JsonSerializer.SerializeToUtf8Bytes(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<RepeatingEntityModel>(v, jsonSerializerOptions)!);
        }
    }
}
