using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduleEntityConfiguration : IEntityTypeConfiguration<ScheduleEntity>
    {
        public void Configure(EntityTypeBuilder<ScheduleEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.EndsOn);

            builder.Property(x => x.UserId).IsRequired();
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
