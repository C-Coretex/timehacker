using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots;

public class ScheduledTaskConfiguration : UserScopedEntityConfigurationBase<ScheduledTask>
{
    public override void Configure(EntityTypeBuilder<ScheduledTask> builder)
    {
        ConfigureUserScoped(builder);

        builder.HasIndex(x => x.IsCompleted);
        builder.Property(x => x.IsCompleted).IsRequired();

        // FK to the parent snapshot is keyed on the composite (UserId, Date) alternate key rather than the
        // snapshot's surrogate Id — a snapshot is uniquely identified by user+day, so its generated tasks
        // attach by user+day too.
        builder.HasOne(x => x.ScheduleSnapshot).WithMany(x => x.ScheduledTasks)
               .HasForeignKey(x => new { x.UserId, x.Date }).HasPrincipalKey(x => new { x.UserId, x.Date })
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ScheduledCategory).WithMany(x => x.ScheduledTasks)
               .HasForeignKey(x => x.ScheduledCategoryId).HasPrincipalKey(x => x.Id)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ScheduleEntity).WithMany(x => x.ScheduledTasks)
                .HasForeignKey(x => x.ParentScheduleEntityId).HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
    }
}
