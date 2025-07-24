using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.IServices.Tasks;

namespace TimeHacker.Domain.Services.Services.Tasks
{
    public class FixedTaskService(IFixedTaskRepository fixedTaskRepository)
        : IFixedTaskService
    {
        public IAsyncEnumerable<FixedTask> GetAll() => fixedTaskRepository.GetAll().AsAsyncEnumerable();

        public Task AddAsync(FixedTask task)
        {
            return fixedTaskRepository.AddAndSaveAsync(task);
        }

        public Task UpdateAsync(FixedTask task)
        {
            if (task == null)
                throw new NotProvidedException(nameof(task));
;
            return fixedTaskRepository.UpdateAndSaveAsync(task);
        }

        public Task DeleteAsync(Guid id)
        {
            return fixedTaskRepository.DeleteAndSaveAsync(id);
        }

        public Task<FixedTask?> GetByIdAsync(Guid id)
        {
            return fixedTaskRepository.GetByIdAsync(id);
        }

        public async Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid taskId)
        {
            if (scheduleEntity == null)
                throw new NotProvidedException(nameof(scheduleEntity));

            var task = await fixedTaskRepository.GetByIdAsync(taskId, false);
            if (task == null)
                throw new NotFoundException(nameof(task), taskId.ToString());

            task.ScheduleEntity = scheduleEntity;
            return (await fixedTaskRepository.UpdateAndSaveAsync(task)).ScheduleEntity!;
        }
    }
}
