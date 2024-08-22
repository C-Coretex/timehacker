using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.IServices.Tasks
{
    public interface IFixedTaskService
    {
        /// <returns>Query with filtration by user id applied.</returns>
        public IQueryable<FixedTask> GetAll();
        Task<FixedTask?> GetByIdAsync(uint id);
        public Task UpdateAsync(FixedTask task);
        public Task DeleteAsync(uint id);
        public Task AddAsync(FixedTask task);
    }
}
