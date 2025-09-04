using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Categories;

namespace TimeHacker.Infrastructure.Repositories.Categories
{
    public class CategoryRepository : UserScopedRepositoryBase<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor) : base(dbContext, dbContext.Category, userAccessor)
        {}
    }
}
