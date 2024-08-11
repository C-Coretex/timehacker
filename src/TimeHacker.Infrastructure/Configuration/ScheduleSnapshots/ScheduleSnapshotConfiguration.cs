using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduleSnapshotConfiguration : IEntityTypeConfiguration<ScheduleSnapshot>
    {
        public void Configure(EntityTypeBuilder<ScheduleSnapshot> builder)
        {
            builder.HasKey(u => new { u.UserId, u.Date });
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.UserId).HasMaxLength(450);

            //Serialize as BSON, since there will be entry for every day for every user.
            //In the future we may want to use MessagePack
            var jsonOptions = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            builder.Property(x => x.ScheduleData).HasConversion(
                x => JsonSerializer.SerializeToUtf8Bytes(x, jsonOptions),
                x => JsonSerializer.Deserialize<TasksForDayReturn>(x, jsonOptions));
        }
    }
}
