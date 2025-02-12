using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Contracts.Entities.Users;

namespace TimeHacker.Infrastructure.Configuration.Users
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(x => x.Id).HasMaxLength(450);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
        }
    }
}
