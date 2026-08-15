using TimeHacker.Domain.Entities.Tags;

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
        NotProvidedException.ThrowIfNull(task);

        return (await dynamicTaskRepository.AddAndSaveAsync(task.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(DynamicTaskDto task, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(task);

        var entity = await dynamicTaskRepository.GetAndUpdateAndSaveAsync(task.Id!.Value, e => task.GetEntity(e), cancellationToken);
        if (entity is null)
            throw new NotFoundException("DynamicTask", task.Id!.Value.ToString());
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await dynamicTaskRepository.DeleteAndSaveAsync(id, cancellationToken))
            throw new NotFoundException("DynamicTask", id.ToString());
    }

    public async Task<DynamicTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dynamicTaskRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        return DynamicTaskDto.Create(entity);
    }
}
