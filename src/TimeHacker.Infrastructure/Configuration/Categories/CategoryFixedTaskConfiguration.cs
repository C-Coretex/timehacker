using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Infrastructure.Configuration.Categories
{
    public class CategoryFixedTaskConfiguration : IEntityTypeConfiguration<CategoryFixedTask>
    {
        public void Configure(EntityTypeBuilder<CategoryFixedTask> builder)
        {
            builder.HasKey(x => new { x.CategoryId, x.FixedTaskId });

            builder.HasOne(x => x.FixedTask).WithMany(x => x.CategoryFixedTasks)
                   .HasForeignKey(x => x.FixedTaskId).HasPrincipalKey(x => x.Id)
                   .OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(x => x.Category).WithMany(x => x.CategoryFixedTasks)
                   .HasForeignKey(x => x.CategoryId).HasPrincipalKey(x => x.Id)
                   .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
