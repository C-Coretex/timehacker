using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Extensions;
using TimeHacker.Application.Api.QueryPipelineSteps;

namespace TimeHacker.Application.Api.AppServices.Tasks;

public class DynamicTaskAppService(IDynamicTaskRepository dynamicTaskRepository, ICategoryRepository categoryRepository)
    : IDynamicTaskAppService
{
    public IAsyncEnumerable<DynamicTaskDto> GetAll(CancellationToken cancellationToken = default)
    {
        return dynamicTaskRepository.GetAll().Select(DynamicTaskDto.Selector).AsAsyncEnumerable();
    }

    public async Task<Guid> AddAsync(DynamicTaskDto task, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(task);
        ThrowIfNoCategories(task.Categories);
        await categoryRepository.ThrowIfAnyCategoryIsNotVisibleAsync(task.Categories, cancellationToken);

        return (await dynamicTaskRepository.AddAndSaveAsync(task.GetEntityWithJunctions(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(DynamicTaskDto task, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(task);
        ThrowIfNoCategories(task.Categories);
        await categoryRepository.ThrowIfAnyCategoryIsNotVisibleAsync(task.Categories, cancellationToken);

        var entity = await dynamicTaskRepository.GetAndUpdateAndSaveAsync(task.Id!.Value, e => task.GetEntityWithJunctions(e), cancellationToken, QueryPipelineDynamicTasks.IncludeCategoryLinks);
        if (entity is null)
            throw new NotFoundException("DynamicTask", task.Id!.Value.ToString());
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await dynamicTaskRepository.DeleteAndSaveAsync(id, cancellationToken))
            throw new NotFoundException("DynamicTask", id.ToString());
    }

    /// <summary>
    /// A dynamic task must carry at least one category — unlike a fixed task, which is free to have none.
    /// Enforced here rather than on the input model so no caller can route around it.
    /// </summary>
    private static void ThrowIfNoCategories(IEnumerable<LinkCategoryDto> categories)
    {
        if (!categories.Any())
            throw new DataIsNotCorrectException("A dynamic task must be linked to at least one category.", nameof(categories));
    }

    public async Task<DynamicTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dynamicTaskRepository.GetByIdAsync(id, cancellationToken: cancellationToken, queryPipelineSteps: QueryPipelineDynamicTasks.IncludeCategoryData);
        return DynamicTaskDto.Create(entity);
    }
}
