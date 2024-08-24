using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.IServices.Categories
{
    public interface ICategoryService
    {
        IQueryable<Category> GetAll();
        Task<Category?> GetByIdAsync(uint id);
        Task UpdateAsync(Category task);
        Task DeleteAsync(uint id);
        Task AddAsync(Category task);

        Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, uint categoryId);
    }
}
