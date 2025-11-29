using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Infrastructure.Configuration.Tasks
{
    public class FixedTaskConfiguration : UserScopedEntityConfigurationBase<FixedTask>
    {
        public override void Configure(EntityTypeBuilder<FixedTask> builder)
        {
            ConfigureUserScoped(builder);

            builder.HasIndex(x => x.CreatedTimestamp);
            builder.HasIndex(x => x.StartTimestamp);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Description).HasMaxLength(516);
            builder.Property(x => x.Priority).IsRequired();
            builder.Property(x => x.StartTimestamp).IsRequired();
            builder.Property(x => x.EndTimestamp).IsRequired();
            builder.Property(x => x.CreatedTimestamp).IsRequired();

            builder.HasOne(x => x.ScheduleEntity).WithOne(x => x.FixedTask)
                .HasForeignKey<FixedTask>(x => x.ScheduleEntityId).HasPrincipalKey<ScheduleEntity>(x => x.Id)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
