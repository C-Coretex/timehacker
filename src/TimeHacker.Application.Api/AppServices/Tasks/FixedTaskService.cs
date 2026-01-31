using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;

namespace TimeHacker.Application.Api.AppServices.Tasks;

public class FixedTaskAppService(IFixedTaskRepository fixedTaskRepository, IScheduleEntityRepository scheduleEntityRepository)
    : IFixedTaskAppService
{
    public IAsyncEnumerable<FixedTaskDto> GetAll() => fixedTaskRepository.GetAll().Select(FixedTaskDto.Selector).AsAsyncEnumerable();

    public async Task<Guid> AddAsync(FixedTaskDto task)
    {
        if (task == null)
            throw new NotProvidedException(nameof(task));

        return (await fixedTaskRepository.AddAndSaveAsync(task.GetEntity())).Id;
    }

    public async Task UpdateAsync(FixedTaskDto task)
    {
        if (task == null)
            throw new NotProvidedException(nameof(task));

        var entity = await fixedTaskRepository.GetByIdAsync(task.Id!.Value);
        await fixedTaskRepository.UpdateAndSaveAsync(task.GetEntity(entity));
    }

    public async Task DeleteAsync(Guid id)
    {
        await scheduleEntityRepository.DeleteBy(x => x.FixedTask != null && x.FixedTask.Id == id);
        await fixedTaskRepository.DeleteAndSaveAsync(id);
    }

    public async Task<FixedTaskDto?> GetByIdAsync(Guid id)
    {
        var entity = await fixedTaskRepository.GetByIdAsync(id);
        return FixedTaskDto.Create(entity);
    }
}