using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

public interface IDynamicTaskAppService
{
    /// <returns>Query with filtration by user id applied.</returns>
    public IAsyncEnumerable<DynamicTaskDto> GetAll();
    Task<DynamicTaskDto?> GetByIdAsync(Guid id);
    public Task UpdateAsync(DynamicTaskDto task);
    public Task DeleteAsync(Guid id);
    public Task AddAsync(DynamicTaskDto task);
}
