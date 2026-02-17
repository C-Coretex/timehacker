using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Application.Api.QueryPipelineSteps;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks;

public class FixedTaskAppService(IFixedTaskRepository fixedTaskRepository, IScheduleEntityRepository scheduleEntityRepository)
    : IFixedTaskAppService
{
    public IAsyncEnumerable<FixedTaskDto> GetAll(CancellationToken cancellationToken = default) =>
        fixedTaskRepository.GetAll().Select(FixedTaskDto.Selector).AsAsyncEnumerable();

    public async Task<Guid> AddAsync(FixedTaskDto task, CancellationToken cancellationToken = default)
    {
        if (task == null)
            throw new NotProvidedException(nameof(task));

        return (await fixedTaskRepository.AddAndSaveAsync(task.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(FixedTaskDto task, CancellationToken cancellationToken = default)
    {
        if (task == null)
            throw new NotProvidedException(nameof(task));

        var entity = await fixedTaskRepository.GetByIdAsync(task.Id!.Value, cancellationToken: cancellationToken);
        await fixedTaskRepository.UpdateAndSaveAsync(task.GetEntity(entity), cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await scheduleEntityRepository.DeleteBy(x => x.FixedTask != null && x.FixedTask.Id == id, cancellationToken);
        await fixedTaskRepository.DeleteAndSaveAsync(id, cancellationToken);
    }

    public async Task<FixedTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await fixedTaskRepository.GetByIdAsync(id, cancellationToken: cancellationToken, queryPipelineSteps: QueryPipelineFixedTasks.IncludeRepeatingData);
        return FixedTaskDto.Create(entity);
    }
}