using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.IRepositories.Categories;
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
        NotProvidedException.ThrowIfNull(task);

        return (await dynamicTaskRepository.AddAndSaveAsync(task.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(DynamicTaskDto task, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(task);

        var entity = await dynamicTaskRepository.GetByIdAsync(task.Id!.Value, asNoTracking: false, cancellationToken: cancellationToken)
                     ?? throw new NotFoundException("DynamicTask", task.Id!.Value.ToString());
        task.GetEntity(entity);

        await dynamicTaskRepository.SaveChangesAsync(cancellationToken);
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
