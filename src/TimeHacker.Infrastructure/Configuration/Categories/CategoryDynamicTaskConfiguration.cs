using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Infrastructure.Configuration.Categories;

public class CategoryDynamicTaskConfiguration : IEntityTypeConfiguration<CategoryDynamicTask>
{
    public void Configure(EntityTypeBuilder<CategoryDynamicTask> builder)
    {
        builder.HasKey(x => new { x.CategoryId, x.DynamicTaskId });
        builder.HasKey(x => new { x.CategoryId, x.DynamicTaskId });

        builder.HasOne(x => x.DynamicTask).WithMany(x => x.CategoryDynamicTasks)
               .HasForeignKey(x => x.DynamicTaskId).HasPrincipalKey(x => x.Id)
               .OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne(x => x.Category).WithMany(x => x.CategoryDynamicTasks)
               .HasForeignKey(x => x.CategoryId).HasPrincipalKey(x => x.Id)
               .OnDelete(DeleteBehavior.ClientCascade);
    }
}
