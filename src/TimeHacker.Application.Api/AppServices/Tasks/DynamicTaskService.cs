using Microsoft.EntityFrameworkCore;
using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks
{
    public class DynamicTaskAppService(IDynamicTaskRepository dynamicTaskRepository)
        : IDynamicTaskAppService
    {
        public IAsyncEnumerable<DynamicTaskDto> GetAll()
        {
            return dynamicTaskRepository.GetAll().Select(DynamicTaskDto.Selector).AsAsyncEnumerable();
        }

        public async Task AddAsync(DynamicTaskDto task)
        {
            if (task == null)
                throw new NotProvidedException(nameof(task));

            await dynamicTaskRepository.AddAndSaveAsync(task.GetEntity());
        }

        public async Task UpdateAsync(DynamicTaskDto task)
        {
            if (task == null)
                throw new NotProvidedException(nameof(task));

            var entity = await dynamicTaskRepository.GetByIdAsync(task.Id!.Value);
            entity = await dynamicTaskRepository.UpdateAndSaveAsync(task.GetEntity(entity));
            await dynamicTaskRepository.UpdateAndSaveAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await dynamicTaskRepository.DeleteAndSaveAsync(id);
        }

        public async Task<DynamicTaskDto?> GetByIdAsync(Guid id)
        {
            var entity = await dynamicTaskRepository.GetByIdAsync(id);
            return DynamicTaskDto.Create(entity);
        }
    }
}
