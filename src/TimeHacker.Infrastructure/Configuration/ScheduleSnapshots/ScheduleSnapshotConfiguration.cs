using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduleSnapshotConfiguration : UserScopedEntityConfigurationBase<ScheduleSnapshot>
    {
        public override void Configure(EntityTypeBuilder<ScheduleSnapshot> builder)
        {
            ConfigureUserScoped(builder);

            builder.HasAlternateKey(u => new { u.UserId, u.Date });

            builder.HasIndex(x => x.Date);
        }
    }
}
