using TimeHacker.Domain.Entities.EntityBase;

namespace TimeHacker.Infrastructure.Configuration;

public abstract class UserScopedEntityConfigurationBase<T> : IEntityTypeConfiguration<T> where T : UserScopedEntityBase
{
    public abstract void Configure(EntityTypeBuilder<T> builder);

    public void ConfigureUserScoped(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.UserId).IsRequired();
    }
}
