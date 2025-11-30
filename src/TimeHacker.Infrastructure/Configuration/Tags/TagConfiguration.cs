using TimeHacker.Domain.Entities.Tags;

namespace TimeHacker.Infrastructure.Configuration.Tags;

public class TagConfiguration : UserScopedEntityConfigurationBase<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        ConfigureUserScoped(builder);

        builder.HasIndex(x => x.Category);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Category).HasMaxLength(64);
        builder.Property(x => x.Color).IsRequired().HasConversion<Converters.ColorConverter> ();
    }
}
