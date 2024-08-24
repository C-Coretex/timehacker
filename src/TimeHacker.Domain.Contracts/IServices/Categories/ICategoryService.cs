using TimeHacker.Domain.Contracts.Entities.Categories;

namespace TimeHacker.Domain.Contracts.IServices.Categories
{
    public interface ICategoryService
    {
        public IQueryable<Category> GetAll();
        Task<Category?> GetByIdAsync(uint id);
        public Task UpdateAsync(Category task);
        public Task DeleteAsync(uint id);
        public Task AddAsync(Category task);
    }
}
