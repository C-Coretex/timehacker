using System.Drawing;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduledCategoryConfiguration : UserScopedEntityConfigurationBase<ScheduledCategory>
    {
        public override void Configure(EntityTypeBuilder<ScheduledCategory> builder)
        {
            ConfigureUserScoped(builder);

            builder.HasOne(x => x.ScheduleSnapshot).WithMany(x => x.ScheduledCategories)
                   .HasForeignKey(x => new { x.UserId, x.Date }).HasPrincipalKey(x => new { x.UserId, x.Date })
                   .OnDelete(DeleteBehavior.ClientCascade);

            builder.Property(x => x.Color).IsRequired().HasConversion(
                v => v.ToArgb(),
                v => Color.FromArgb(v)
            );

            builder.HasOne(x => x.ScheduleEntity).WithMany(x => x.ScheduledCategories)
                    .HasForeignKey(x => x.ParentScheduleEntity).HasPrincipalKey(x => x.Id)
                    .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
