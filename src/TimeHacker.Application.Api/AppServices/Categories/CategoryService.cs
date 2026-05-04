using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;
using TimeHacker.Domain.IRepositories.Categories;

namespace TimeHacker.Application.Api.AppServices.Categories;

public class CategoryService(ICategoryRepository categoryRepository)
    : ICategoryAppService
{
    public IAsyncEnumerable<CategoryDto> GetAll(CancellationToken cancellationToken = default) => categoryRepository.GetAll(true).Select(CategoryDto.Selector).AsAsyncEnumerable();

    public async Task<Guid> AddAsync(CategoryDto category, CancellationToken cancellationToken = default)
    {
        if (category == null)
            throw new NotProvidedException(nameof(category));

        return (await categoryRepository.AddAndSaveAsync(category.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(CategoryDto category, CancellationToken cancellationToken = default)
    {
        if (category == null)
            throw new NotProvidedException(nameof(category));

        var entity = await categoryRepository.GetByIdAsync(category.Id!.Value, cancellationToken: cancellationToken);
        await categoryRepository.UpdateAndSaveAsync(category.GetEntity(entity), cancellationToken);
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return categoryRepository.DeleteAndSaveAsync(id, cancellationToken);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await categoryRepository.GetAll(true).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity != null ? CategoryDto.Create(entity) : null;
    }
}
