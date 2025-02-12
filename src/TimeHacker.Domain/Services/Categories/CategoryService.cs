using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IServices.Categories;

namespace TimeHacker.Domain.Services.Categories
{
    public class CategoryService: ICategoryService
    {
        private readonly UserAccessorBase _userAccessorBase;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, UserAccessorBase userAccessorBase)
        {
            _userAccessorBase = userAccessorBase;
            _categoryRepository = categoryRepository;
        }

        public async Task AddAsync(Category category)
        {
            var userId = _userAccessorBase.UserId!;
            category.UserId = userId;

            await _categoryRepository.AddAsync(category);
        }
        public async Task UpdateAsync(Category category)
        {
            var userId = _userAccessorBase.UserId;

            if (category == null)
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
        public async Task DeleteAsync(Guid id)
        {
            var userId = _userAccessorBase.UserId;
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return;

            if (category.UserId != userId)
                throw new ArgumentException("User can only delete its own categories.");

            await _categoryRepository.DeleteAsync(category);
        }

        public IQueryable<Category> GetAll() => GetAll(true);

        public Task<Category?> GetByIdAsync(Guid id)
        {
            return GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid categoryId)
        {
            var userId = _userAccessorBase.UserId!;
            if (scheduleEntity == null)
                throw new ArgumentException("Values are incorrect.");

            var task = await GetAll(false).FirstOrDefaultAsync(x => x.Id == categoryId);
            if (task == null)
                throw new Exception("Category by this Id is not found for current user.");

            task.ScheduleEntity = scheduleEntity;
            return (await _categoryRepository.UpdateAsync(task)).ScheduleEntity!;
        }

        private IQueryable<Category> GetAll(bool asNoTracking)
        {
            var userId = _userAccessorBase.UserId;
            return _categoryRepository.GetAll(asNoTracking).Where(x => x.UserId == userId);
        }
    }
}
