using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Application.Api.QueryPipelineSteps;
using TimeHacker.Domain.Entities.Tags;
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
        NotProvidedException.ThrowIfNull(task);

        return (await fixedTaskRepository.AddAndSaveAsync(task.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(FixedTaskDto task, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(task);

        var entity = await fixedTaskRepository.GetByIdAsync(task.Id!.Value, asNoTracking: false, cancellationToken: cancellationToken)
                     ?? throw new NotFoundException("FixedTask", task.Id!.Value.ToString());
        task.GetEntity(entity);

        await fixedTaskRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // The FK lives on FixedTask.ScheduleEntityId pointing TO ScheduleEntity, so the DB cascade only fires
        // entity->task, not task->entity. Delete the owning ScheduleEntity explicitly first to avoid orphaning it.
        await scheduleEntityRepository.DeleteBy(x => x.FixedTask != null && x.FixedTask.Id == id, cancellationToken);
        await fixedTaskRepository.DeleteAndSaveAsync(id, cancellationToken);
    }

    public async Task<FixedTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await fixedTaskRepository.GetByIdAsync(id, cancellationToken: cancellationToken, queryPipelineSteps: QueryPipelineFixedTasks.IncludeRepeatingData);
        return FixedTaskDto.Create(entity);
    }
}