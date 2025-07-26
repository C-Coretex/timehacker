using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Categories
{
    public interface ICategoryAppService
    {
        IQueryable<Category> GetAll();
        Task<Category?> GetByIdAsync(Guid id);
        Task UpdateAsync(Category task);
        Task DeleteAsync(Guid id);
        Task AddAsync(Category task);
    }
}
