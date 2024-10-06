using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Contracts.Entities.Tags;

namespace TimeHacker.Infrastructure.Configuration.Tags
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Category);

            builder.Property(x => x.UserId).HasMaxLength(450);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
            builder.Property(x => x.Category).HasMaxLength(64);
            builder.Property(x => x.Color).IsRequired().HasConversion<Converters.ColorConverter> ();
        }
    }
}
