using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Categories;

public interface ICategoryScheduleAppService
{
    IAsyncEnumerable<CategoryScheduleDto> GetAllForCategory(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(CategoryScheduleDto categorySchedule, CancellationToken cancellationToken = default);
    Task UpdateAsync(CategoryScheduleDto categorySchedule, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid categoryId, Guid id, CancellationToken cancellationToken = default);
}
