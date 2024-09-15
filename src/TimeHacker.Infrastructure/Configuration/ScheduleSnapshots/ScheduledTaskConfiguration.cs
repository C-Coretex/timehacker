using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduledTaskConfiguration : IEntityTypeConfiguration<ScheduledTask>
    {
        public void Configure(EntityTypeBuilder<ScheduledTask> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.IsCompleted);
            builder.Property(x => x.IsCompleted).IsRequired();

            builder.HasOne(x => x.ScheduleSnapshot).WithMany(x => x.ScheduledTasks)
                   .HasForeignKey(x => new { x.UserId, x.Date }).HasPrincipalKey(x => new { x.UserId, x.Date })
                   .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(x => x.ScheduledCategory).WithMany(x => x.ScheduledTasks)
                   .HasForeignKey(x => x.ScheduledCategoryId).HasPrincipalKey(x => x.Id)
                   .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(x => x.ScheduleEntity).WithMany(x => x.ScheduledTasks)
                    .HasForeignKey(x => x.ParentScheduleEntityId).HasPrincipalKey(x => x.Id)
                    .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
