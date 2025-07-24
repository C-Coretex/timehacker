using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IServices.Categories;

namespace TimeHacker.Domain.Services.Services.Categories
{
    public class CategoryService(ICategoryRepository categoryRepository)
        : ICategoryService
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

        public async Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid categoryId)
        {
            if (scheduleEntity == null)
                throw new NotProvidedException(nameof(scheduleEntity));

            var task = await categoryRepository.GetByIdAsync(categoryId, false);
            if (task == null)
                throw new NotFoundException(nameof(task), categoryId.ToString());

            task.ScheduleEntity = scheduleEntity;
            return (await categoryRepository.UpdateAndSaveAsync(task)).ScheduleEntity!;

        }

    }
}
