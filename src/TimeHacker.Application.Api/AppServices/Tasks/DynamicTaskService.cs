using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks;

public class DynamicTaskAppService(IDynamicTaskRepository dynamicTaskRepository)
    : IDynamicTaskAppService
{
    public IAsyncEnumerable<DynamicTaskDto> GetAll(CancellationToken cancellationToken = default)
    {
        return dynamicTaskRepository.GetAll().Select(DynamicTaskDto.Selector).AsAsyncEnumerable();
    }

    public async Task<Guid> AddAsync(DynamicTaskDto task, CancellationToken cancellationToken = default)
    {
        if (task == null)
            throw new NotProvidedException(nameof(task));

        return (await dynamicTaskRepository.AddAndSaveAsync(task.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(DynamicTaskDto task, CancellationToken cancellationToken = default)
    {
        if (task == null)
            throw new NotProvidedException(nameof(task));

        var entity = await dynamicTaskRepository.GetByIdAsync(task.Id!.Value, cancellationToken: cancellationToken);
        entity = await dynamicTaskRepository.UpdateAndSaveAsync(task.GetEntity(entity), cancellationToken);
        await dynamicTaskRepository.UpdateAndSaveAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dynamicTaskRepository.DeleteAndSaveAsync(id, cancellationToken);
    }

    public async Task<DynamicTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dynamicTaskRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        return DynamicTaskDto.Create(entity);
    }
}
