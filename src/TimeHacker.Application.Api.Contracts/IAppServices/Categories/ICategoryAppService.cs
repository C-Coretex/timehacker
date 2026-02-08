using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Categories;

public interface ICategoryAppService
{
    IAsyncEnumerable<CategoryDto> GetAll(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(CategoryDto task, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(CategoryDto task, CancellationToken cancellationToken = default);
}
