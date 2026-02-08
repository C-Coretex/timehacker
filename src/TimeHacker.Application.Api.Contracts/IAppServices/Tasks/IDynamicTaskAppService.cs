using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

public interface IDynamicTaskAppService
{
    /// <returns>Query with filtration by user id applied.</returns>
    public IAsyncEnumerable<DynamicTaskDto> GetAll(CancellationToken cancellationToken = default);
    Task<DynamicTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task UpdateAsync(DynamicTaskDto task, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<Guid> AddAsync(DynamicTaskDto task, CancellationToken cancellationToken = default);
}
