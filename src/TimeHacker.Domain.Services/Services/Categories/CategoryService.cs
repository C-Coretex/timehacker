using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IServices.Categories;

namespace TimeHacker.Domain.Services.Services.Categories
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
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            category.UserId = userId;

            await _categoryRepository.AddAndSaveAsync(category);
        }
        public async Task UpdateAsync(Category category)
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();

            if (category == null)
                throw new NotProvidedException(nameof(category));


            var oldCategory = await _categoryRepository.GetByIdAsync(category.Id);
            if (oldCategory == null)
            {
                await _categoryRepository.AddAndSaveAsync(category);
                return;
            }

            //TODO: will be removed after repository level filtrations, it will just be null and another exception will be thrown
            if (oldCategory.UserId != userId)
                throw new ArgumentException("User can only edit its own categories.");

            category.UserId = userId;
            await _categoryRepository.UpdateAndSaveAsync(category);
        }
        public async Task DeleteAsync(Guid id)
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return;

            //TODO: will be removed after repository level filtrations, it will just be null and another exception will be thrown
            if (category.UserId != userId)
                throw new ArgumentException("User can only delete its own categories.");

            await _categoryRepository.DeleteAndSaveAsync(category);
        }

        public IQueryable<Category> GetAll() => GetAll(true);

        public Task<Category?> GetByIdAsync(Guid id)
        {
            return GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid categoryId)
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized()!;
            if (scheduleEntity == null)
                throw new NotProvidedException(nameof(scheduleEntity));

            var task = await GetAll(false).FirstOrDefaultAsync(x => x.Id == categoryId);
            if (task == null)
                throw new NotFoundException(nameof(task), categoryId.ToString());

            task.ScheduleEntity = scheduleEntity;
            return (await _categoryRepository.UpdateAndSaveAsync(task)).ScheduleEntity!;
        }

        private IQueryable<Category> GetAll(bool asNoTracking)
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            return _categoryRepository.GetAll(asNoTracking).Where(x => x.UserId == userId);
        }
    }
}
