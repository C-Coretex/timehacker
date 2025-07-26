using Microsoft.EntityFrameworkCore;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks
{
    public class FixedTaskAppService(IFixedTaskRepository fixedTaskRepository)
        : IFixedTaskAppService
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
    }
}
