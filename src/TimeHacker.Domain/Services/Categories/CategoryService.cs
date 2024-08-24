using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IServices.Categories;

namespace TimeHacker.Domain.Services.Categories
{
    public class CategoryService: ICategoryService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(IUserAccessor userAccessor, ICategoryRepository categoryRepository)
        {
            _userAccessor = userAccessor;
            _categoryRepository = categoryRepository;
        }

        public async Task AddAsync(Category category)
        {
            var userId = _userAccessor.UserId;
            category.UserId = userId;

            await _categoryRepository.AddAsync(category);
        }

        public async Task DeleteAsync(uint id)
        {
            var userId = _userAccessor.UserId;
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return;

            if (category.UserId != userId)
                throw new ArgumentException("User can only delete its own categories.");

            await _categoryRepository.DeleteAsync(category);
        }

        public IQueryable<Category> GetAll()
        {
            var userId = _userAccessor.UserId;
            return _categoryRepository.GetAll().Where(x => x.UserId == userId);
        }

        public Task<Category?> GetByIdAsync(uint id)
        {
            return GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Category category)
        {
            var userId = _userAccessor.UserId;

            if (category == null || category.Id == 0)
                throw new ArgumentException("Category must be valid");


            var oldCategory = await _categoryRepository.GetByIdAsync(category.Id);
            if (oldCategory == null)
            {
                await _categoryRepository.AddAsync(category);
                return;
            }

            if (oldCategory.UserId != userId)
                throw new ArgumentException("User can only edit its own categories.");

            category.UserId = userId;
            await _categoryRepository.UpdateAsync(category);
        }
    }
}
