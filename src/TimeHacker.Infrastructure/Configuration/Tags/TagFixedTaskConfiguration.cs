using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Entities.Tags;

namespace TimeHacker.Infrastructure.Configuration.Tags
{
    public class TagFixedTaskConfiguration : IEntityTypeConfiguration<TagFixedTask>
    {
        public void Configure(EntityTypeBuilder<TagFixedTask> builder)
        {
            builder.HasKey(x => new { x.TagId, x.TaskId });

            builder.HasOne(x => x.Tag).WithMany(x => x.TagFixedTasks)
                .HasForeignKey(x => x.TagId).HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(x => x.Task).WithMany(x => x.TagFixedTasks)
                .HasForeignKey(x => x.TaskId).HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
