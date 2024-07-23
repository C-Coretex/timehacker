using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Helpers.DB.Abstractions.BaseClasses;

namespace TimeHacker.Infrastructure.Repositories.Categories
{
    internal class CategoryRepository : RepositoryBase<TimeHackerDbContext, Category, int>, ICategoryRepository
    {
        public CategoryRepository(TimeHackerDbContext dbContext) : base(dbContext, dbContext.Category)
        {}
    }
}
