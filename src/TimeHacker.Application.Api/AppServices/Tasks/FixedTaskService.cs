using TimeHacker.Application.Api.QueryPipelineSteps;
using TimeHacker.Domain.Entities.Tags;

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

        var entity = await fixedTaskRepository.GetAndUpdateAndSaveAsync(task.Id!.Value, e => task.GetEntity(e), cancellationToken);
        if (entity is null)
            throw new NotFoundException("FixedTask", task.Id!.Value.ToString());
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // FixedTask.ScheduleEntityId points TO ScheduleEntity, so deleting the owning ScheduleEntity cascade-deletes
        // this FixedTask (entity->task). Delete the schedule first — its predicate needs the FixedTask navigation to
        // still exist. If a schedule existed, that cascade already removed the task (so the task delete finds
        // nothing); if not, the task delete does the removal. The id was genuinely absent only if neither removed
        // anything, in which case it's a 404.
        var schedulesDeleted = await scheduleEntityRepository.DeleteBy(x => x.FixedTask != null && x.FixedTask.Id == id, cancellationToken);
        var taskDeleted = await fixedTaskRepository.DeleteAndSaveAsync(id, cancellationToken);

        if (schedulesDeleted == 0 && !taskDeleted)
            throw new NotFoundException("FixedTask", id.ToString());
    }

    public async Task<FixedTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await fixedTaskRepository.GetByIdAsync(id, cancellationToken: cancellationToken, queryPipelineSteps: QueryPipelineFixedTasks.IncludeRepeatingData);
        return FixedTaskDto.Create(entity);
    }
}