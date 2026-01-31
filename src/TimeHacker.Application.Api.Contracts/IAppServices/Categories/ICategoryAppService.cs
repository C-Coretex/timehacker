using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Categories;

public interface ICategoryAppService
{
    IAsyncEnumerable<CategoryDto> GetAll();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task UpdateAsync(CategoryDto task);
    Task DeleteAsync(Guid id);
    Task<Guid> AddAsync(CategoryDto task);
}
