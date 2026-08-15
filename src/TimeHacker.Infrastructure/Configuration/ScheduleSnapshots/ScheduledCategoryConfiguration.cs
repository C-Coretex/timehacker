using System.Drawing;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots;

public class ScheduledCategoryConfiguration : UserScopedEntityConfigurationBase<ScheduledCategory>
{
    public override void Configure(EntityTypeBuilder<ScheduledCategory> builder)
    {
        ConfigureUserScoped(builder);

        // FK to the parent snapshot is keyed on the composite (UserId, Date) alternate key rather than the
        // snapshot's surrogate Id — a snapshot is uniquely identified by user+day.
        builder.HasOne(x => x.ScheduleSnapshot).WithMany(x => x.ScheduledCategories)
               .HasForeignKey(x => new { x.UserId, x.Date }).HasPrincipalKey(x => new { x.UserId, x.Date })
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Color).IsRequired().HasConversion(
            v => v.ToArgb(),
            v => Color.FromArgb(v)
        );

        builder.HasOne(x => x.ScheduleEntity).WithMany(x => x.ScheduledCategories)
                .HasForeignKey(x => x.ParentScheduleEntity).HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
    }
}
