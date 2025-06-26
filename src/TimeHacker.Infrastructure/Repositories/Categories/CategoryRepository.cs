using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.Categories
{
    public class CategoryRepository : RepositoryBase<TimeHackerDbContext, Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.Category)
        {}
    }
}
