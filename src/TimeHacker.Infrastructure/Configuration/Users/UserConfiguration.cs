using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Entities.Users;

namespace TimeHacker.Infrastructure.Configuration.Users
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(x => x.Id);

            builder.Property(x => x.IdentityId).HasMaxLength(450).IsRequired();
            builder.HasIndex(x => x.IdentityId).IsUnique();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
        }
    }
}
