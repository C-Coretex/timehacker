using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.IServices.Tasks;

namespace TimeHacker.Domain.Services.Services.Tasks
{
    public class DynamicTaskService(IDynamicTaskRepository dynamicTaskRepository)
        : IDynamicTaskService
    {
        public IAsyncEnumerable<DynamicTask> GetAll()
        {
            return dynamicTaskRepository.GetAll().AsAsyncEnumerable();
        }

        public async Task AddAsync(DynamicTask task)
        {
            await dynamicTaskRepository.AddAndSaveAsync(task);
        }

        public async Task UpdateAsync(DynamicTask task)
        {
            if (task == null)
                throw new NotProvidedException(nameof(task));

            await dynamicTaskRepository.UpdateAndSaveAsync(task);
        }

        public async Task DeleteAsync(Guid id)
        {
            await dynamicTaskRepository.DeleteAndSaveAsync(id);
        }

        public Task<DynamicTask?> GetByIdAsync(Guid id)
        {
            return dynamicTaskRepository.GetByIdAsync(id);
        }
    }
}
