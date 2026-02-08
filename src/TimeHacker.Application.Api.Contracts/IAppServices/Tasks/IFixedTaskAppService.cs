using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

public interface IFixedTaskAppService
{
    /// <returns>Query with filtration by user id applied.</returns>
    IAsyncEnumerable<FixedTaskDto> GetAll(CancellationToken cancellationToken = default);
    Task<FixedTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(FixedTaskDto task, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(FixedTaskDto task, CancellationToken cancellationToken = default);
}
