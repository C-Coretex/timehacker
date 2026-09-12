namespace TimeHacker.Infrastructure.Configuration.Categories;

internal sealed class CategoryScheduleConfiguration : UserScopedEntityConfigurationBase<CategorySchedule>
{
    public override void Configure(EntityTypeBuilder<CategorySchedule> builder)
    {
        ConfigureUserScoped(builder);

        builder.Property(x => x.Description).HasMaxLength(516);

        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.StartTime).IsRequired();
        builder.Property(x => x.EndTime).IsRequired();

        // Deliberately not unique: one category may own several windows on the same day, and they are
        // allowed to overlap. This only serves TaskService's per-day lookup on a timeline miss.
        builder.HasIndex(x => new { x.UserId, x.Date });

        builder.HasOne(x => x.Category).WithMany(x => x.Schedules)
            .HasForeignKey(x => x.CategoryId).HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ScheduleEntity).WithOne(x => x.CategorySchedule)
            .HasForeignKey<CategorySchedule>(x => x.ScheduleEntityId).HasPrincipalKey<ScheduleEntity>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
