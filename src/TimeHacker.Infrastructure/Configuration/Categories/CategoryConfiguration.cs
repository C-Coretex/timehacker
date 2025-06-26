using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Drawing;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.Categories
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.UserId).HasMaxLength(450);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Description).HasMaxLength(516);
            builder.Property(x => x.Color).IsRequired().HasConversion<Converters.ColorConverter>();

            builder.HasOne(x => x.ScheduleEntity).WithOne(x => x.Category)
                .HasForeignKey<Category>(x => x.ScheduleEntityId).HasPrincipalKey<ScheduleEntity>(x => x.Id)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
