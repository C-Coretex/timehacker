using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Infrastructure.Configuration.Categories
{
    public class CategoryConfiguration : UserScopedEntityConfigurationBase<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            ConfigureUserScoped(builder);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Description).HasMaxLength(516);
            builder.Property(x => x.Color).IsRequired().HasConversion<Converters.ColorConverter>();

            builder.HasOne(x => x.ScheduleEntity).WithOne(x => x.Category)
                .HasForeignKey<Category>(x => x.ScheduleEntityId).HasPrincipalKey<ScheduleEntity>(x => x.Id)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
