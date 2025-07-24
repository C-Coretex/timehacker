using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.IServices.Tasks
{
    public interface IFixedTaskService
    {
        /// <returns>Query with filtration by user id applied.</returns>
        IAsyncEnumerable<FixedTask> GetAll();
        Task<FixedTask?> GetByIdAsync(Guid id);
        Task UpdateAsync(FixedTask task);
        Task DeleteAsync(Guid id);
        Task AddAsync(FixedTask task);

        Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid taskId);
    }
}
