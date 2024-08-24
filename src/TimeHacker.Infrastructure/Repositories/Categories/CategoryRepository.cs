using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Helpers.DB.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.Categories
{
    public class CategoryRepository : RepositoryBase<TimeHackerDbContext, Category, uint>, ICategoryRepository
    {
        public CategoryRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.Category)
        {}
    }
}
