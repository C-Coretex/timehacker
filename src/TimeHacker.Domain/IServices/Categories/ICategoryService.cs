using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.IServices.Categories
{
    public interface ICategoryService
    {
        IQueryable<Category> GetAll();
        Task<Category?> GetByIdAsync(Guid id);
        Task UpdateAsync(Category task);
        Task DeleteAsync(Guid id);
        Task AddAsync(Category task);

        Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid categoryId);
    }
}
