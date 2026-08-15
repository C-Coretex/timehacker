using TimeHacker.Domain.IRepositories.Categories;

namespace TimeHacker.Infrastructure.Repositories.Categories;

internal sealed class CategoryRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider) 
    : UserScopedRepositoryBase<Category, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.Category, userAccessor, timeProvider), ICategoryRepository
{
}
