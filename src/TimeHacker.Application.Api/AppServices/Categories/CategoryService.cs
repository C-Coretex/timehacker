using Microsoft.EntityFrameworkCore;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.IRepositories.Categories;

namespace TimeHacker.Application.Api.AppServices.Categories
{
    public class CategoryService(ICategoryRepository categoryRepository)
        : ICategoryAppService
    {
        public Task AddAsync(Category category)
        {
            return categoryRepository.AddAndSaveAsync(category);
        }

        public Task UpdateAsync(Category category)
        {
            if (category == null)
                throw new NotProvidedException(nameof(category));

            return categoryRepository.UpdateAndSaveAsync(category);
        }

        public Task DeleteAsync(Guid id)
        {
            return categoryRepository.DeleteAndSaveAsync(id);
        }

        public IQueryable<Category> GetAll() => categoryRepository.GetAll(true);

        public Task<Category?> GetByIdAsync(Guid id)
        {
            return categoryRepository.GetAll(true).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
