using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Infrastructure.Configuration.Tasks
{
    public class DynamicTaskConfiguration : UserScopedEntityConfigurationBase<DynamicTask>
    {
        public override void Configure(EntityTypeBuilder<DynamicTask> builder)
        {
            ConfigureUserScoped(builder);

            builder.HasIndex(x => x.CreatedTimestamp);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Description).HasMaxLength(516);
            builder.Property(x => x.Priority).IsRequired();
            builder.Property(x => x.MinTimeToFinish).IsRequired();
            builder.Property(x => x.MaxTimeToFinish).IsRequired();
            builder.Property(x => x.CreatedTimestamp).IsRequired();
        }
    }
}
