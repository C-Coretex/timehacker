using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Infrastructure.Configuration.Tasks
{
    public class FixedTaskConfiguration : IEntityTypeConfiguration<FixedTask>
    {
        public void Configure(EntityTypeBuilder<FixedTask> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.IsCompleted);
            builder.HasIndex(x => x.CreatedTimestamp);
            builder.HasIndex(x => x.StartTimestamp);

            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Description).HasMaxLength(516);
            builder.Property(x => x.Priority).IsRequired();
            builder.Property(x => x.StartTimestamp).IsRequired();
            builder.Property(x => x.EndTimestamp).IsRequired();
            builder.Property(x => x.IsCompleted).IsRequired();
            builder.Property(x => x.CreatedTimestamp).IsRequired();
        }
    }
}
