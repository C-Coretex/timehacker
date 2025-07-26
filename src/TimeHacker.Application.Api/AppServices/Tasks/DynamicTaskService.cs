using Microsoft.EntityFrameworkCore;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks
{
    public class DynamicTaskAppService(IDynamicTaskRepository dynamicTaskRepository)
        : IDynamicTaskAppService
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
