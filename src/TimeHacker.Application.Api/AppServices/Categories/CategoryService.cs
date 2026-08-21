using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;

namespace TimeHacker.Application.Api.AppServices.Categories;

public class CategoryService(ICategoryRepository categoryRepository)
    : ICategoryAppService
{
    public IAsyncEnumerable<CategoryDto> GetAll(CancellationToken cancellationToken = default) => categoryRepository.GetAll(true).Select(CategoryDto.Selector).AsAsyncEnumerable();

    public async Task<Guid> AddAsync(CategoryDto category, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(category);

        return (await categoryRepository.AddAndSaveAsync(category.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(CategoryDto category, CancellationToken cancellationToken = default)
    {
        NotProvidedException.ThrowIfNull(category);

        var entity = await categoryRepository.GetAndUpdateAndSaveAsync(category.Id!.Value, e => category.GetEntity(e), cancellationToken);
        if (entity is null)
            throw new NotFoundException("Category", category.Id!.Value.ToString());
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if(!await categoryRepository.DeleteAndSaveAsync(id, cancellationToken))
            throw new NotFoundException("Category", id.ToString());
    }

    // Projects through the Selector rather than loading the entity and mapping it, so the linked
    // ScheduleEntity comes back in the same query instead of as a silent null.
    public Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        categoryRepository.GetAll()
            .Where(x => x.Id == id)
            .Select(CategoryDto.Selector)
            .FirstOrDefaultAsync(cancellationToken);
}
