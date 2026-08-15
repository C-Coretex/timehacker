namespace TimeHacker.Infrastructure.Configuration;

public abstract class UserScopedEntityConfigurationBase<T> : IEntityTypeConfiguration<T> where T : UserScopedEntityBase
{
    public abstract void Configure(EntityTypeBuilder<T> builder);

    public void ConfigureUserScoped(EntityTypeBuilder<T> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(u => u.Id);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.UserId).IsRequired();

        // These annotations are consumed by RlsMigrationsModelDiffer to generate the per-table Row-Level
        // Security policy (USING UserId = current user). Marking the entity here is what enables DB-side
        // user isolation; the runtime code does not add a WHERE UserId filter.
        builder.HasAnnotation("Rls:Enabled", true);
        builder.HasAnnotation("Rls:TenantColumn", nameof(UserScopedEntityBase.UserId));
    }
}
