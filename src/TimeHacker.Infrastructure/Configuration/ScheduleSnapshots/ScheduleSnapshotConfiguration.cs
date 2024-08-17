using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduleSnapshotConfiguration : IEntityTypeConfiguration<ScheduleSnapshot>
    {
        public void Configure(EntityTypeBuilder<ScheduleSnapshot> builder)
        {
            builder.HasKey(u => new { u.UserId, u.Date });
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Date);

            builder.Property(x => x.UserId).HasMaxLength(450);
        }
    }
}
