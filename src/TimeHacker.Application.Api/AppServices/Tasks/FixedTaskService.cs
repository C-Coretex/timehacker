using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks;

public class FixedTaskAppService(IFixedTaskRepository fixedTaskRepository)
    : IFixedTaskAppService
{
    public IAsyncEnumerable<FixedTaskDto> GetAll() => fixedTaskRepository.GetAll().Select(FixedTaskDto.Selector).AsAsyncEnumerable();

    public Task AddAsync(FixedTaskDto task)
    {
        if (task == null)
            throw new NotProvidedException(nameof(task));

        return fixedTaskRepository.AddAndSaveAsync(task.GetEntity());
    }

    public async Task UpdateAsync(FixedTaskDto task)
    {
        if (task == null)
            throw new NotProvidedException(nameof(task));

        var entity = await fixedTaskRepository.GetByIdAsync(task.Id!.Value);
        await fixedTaskRepository.UpdateAndSaveAsync(task.GetEntity(entity));
    }

    public Task DeleteAsync(Guid id)
    {
        return fixedTaskRepository.DeleteAndSaveAsync(id);
    }

    public async Task<FixedTaskDto?> GetByIdAsync(Guid id)
    {
        var entity = await fixedTaskRepository.GetByIdAsync(id);
        return FixedTaskDto.Create(entity);
    }
}