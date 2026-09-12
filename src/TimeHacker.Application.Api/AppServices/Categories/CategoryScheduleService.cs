using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;

namespace TimeHacker.Application.Api.AppServices.Categories;

public class CategoryScheduleAppService(
    ICategoryScheduleRepository categoryScheduleRepository,
    ICategoryRepository categoryRepository,
    IScheduleEntityRepository scheduleEntityRepository)
    : ICategoryScheduleAppService
{
    public IAsyncEnumerable<CategoryScheduleDto> GetAllForCategory(Guid categoryId, CancellationToken cancellationToken = default) =>
        categoryScheduleRepository.GetAll()
            .Where(x => x.CategoryId == categoryId)
            .OrderBy(x => x.Date).ThenBy(x => x.StartTime)
            .Select(CategoryScheduleDto.Selector)
            .AsAsyncEnumerable();

    public async Task<Guid> AddAsync(CategoryScheduleDto categorySchedule, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(categorySchedule);

        if (!await categoryRepository.ExistsAsync(categorySchedule.CategoryId, cancellationToken))
            throw new NotFoundException("Category", categorySchedule.CategoryId.ToString());

        return (await categoryScheduleRepository.AddAndSaveAsync(categorySchedule.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(CategoryScheduleDto categorySchedule, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(categorySchedule);

        var entity = await categoryScheduleRepository.GetAndUpdateAndSaveAsync(categorySchedule.Id!.Value, e => categorySchedule.GetEntity(e), cancellationToken) 
            ?? throw new NotFoundException("CategorySchedule", categorySchedule.Id!.Value.ToString());
    }

    public async Task DeleteAsync(Guid categoryId, Guid id, CancellationToken cancellationToken = default)
    {
        // CategorySchedule.ScheduleEntityId points TO ScheduleEntity, so deleting the owning ScheduleEntity
        // cascade-deletes this window. Delete the schedule first — its predicate needs the navigation to still
        // exist — then the window itself; only if neither removed anything was the id genuinely absent.
        // The categoryId is part of the predicate so a window cannot be deleted through another category's route.
        //TODO: add a transaction
        var schedulesDeleted = await scheduleEntityRepository.DeleteBy(
            x => x.CategorySchedule != null && x.CategorySchedule.Id == id && x.CategorySchedule.CategoryId == categoryId,
            cancellationToken);
        var scheduleDeleted = await categoryScheduleRepository.DeleteBy(
            x => x.Id == id && x.CategoryId == categoryId,
            cancellationToken);

        if (schedulesDeleted == 0 && scheduleDeleted == 0)
            throw new NotFoundException("CategorySchedule", id.ToString());
    }
}
