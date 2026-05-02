using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.IRepositories.Categories;

namespace TimeHacker.Infrastructure.Repositories.Categories;

internal sealed class CategoryRepository : UserScopedRepositoryBase<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) 
        : base(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.Category, userAccessor)
    {}
}
