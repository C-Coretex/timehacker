using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.IServices.Tasks
{
    public interface IDynamicTaskService
    {
        /// <returns>Query with filtration by user id applied.</returns>
        public IQueryable<DynamicTask> GetAll();
        Task<DynamicTask?> GetByIdAsync(uint id);
        public Task UpdateAsync(DynamicTask task);
        public Task DeleteAsync(uint id);
        public Task AddAsync(DynamicTask task);
    }
}
