using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.IServices.Tasks
{
    public interface IFixedTaskService
    {
        /// <returns>Query with filtration by user id applied.</returns>
        IQueryable<FixedTask> GetAll();
        Task<FixedTask?> GetByIdAsync(Guid id);
        Task UpdateAsync(FixedTask task);
        Task DeleteAsync(Guid id);
        Task AddAsync(FixedTask task);

        Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid taskId);
    }
}
