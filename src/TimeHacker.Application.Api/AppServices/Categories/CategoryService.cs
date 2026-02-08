using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;
using TimeHacker.Domain.IRepositories.Categories;

namespace TimeHacker.Application.Api.AppServices.Categories;

public class CategoryService(ICategoryRepository categoryRepository)
    : ICategoryAppService
{
    public IAsyncEnumerable<CategoryDto> GetAll(CancellationToken cancellationToken = default) => categoryRepository.GetAll(true).Select(CategoryDto.Selector).AsAsyncEnumerable();

    public async Task<Guid> AddAsync(CategoryDto categoryDto, CancellationToken cancellationToken = default)
    {
        if (categoryDto == null)
            throw new NotProvidedException(nameof(categoryDto));

        return (await categoryRepository.AddAndSaveAsync(categoryDto.GetEntity(), cancellationToken)).Id;
    }

    public async Task UpdateAsync(CategoryDto categoryDto, CancellationToken cancellationToken = default)
    {
        if (categoryDto == null)
            throw new NotProvidedException(nameof(categoryDto));

        var entity = await categoryRepository.GetByIdAsync(categoryDto.Id!.Value, cancellationToken: cancellationToken);
        await categoryRepository.UpdateAndSaveAsync(categoryDto.GetEntity(entity), cancellationToken);
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
