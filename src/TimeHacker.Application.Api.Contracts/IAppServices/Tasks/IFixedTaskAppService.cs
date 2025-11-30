using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

public interface IFixedTaskAppService
{
    /// <returns>Query with filtration by user id applied.</returns>
    IAsyncEnumerable<FixedTaskDto> GetAll();
    Task<FixedTaskDto?> GetByIdAsync(Guid id);
    Task UpdateAsync(FixedTaskDto task);
    Task DeleteAsync(Guid id);
    Task AddAsync(FixedTaskDto task);
}
