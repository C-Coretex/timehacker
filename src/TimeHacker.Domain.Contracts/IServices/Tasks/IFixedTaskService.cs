using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.IServices.Tasks
{
    public interface IFixedTaskService
    {
        /// <returns>Query with filtration by user id applied.</returns>
        IQueryable<FixedTask> GetAll();
        Task<FixedTask?> GetByIdAsync(uint id);
        Task UpdateAsync(FixedTask task);
        Task DeleteAsync(uint id);
        Task AddAsync(FixedTask task);

        Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, uint taskId);
    }
}
