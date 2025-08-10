using TimeHacker.Application.Api.Contracts.DTOs.Tags;
using TimeHacker.Domain.Entities.Tags;

namespace TimeHacker.Application.Api.Contracts.IAppServices.Tags
{
    public interface ITagAppService
    {
        IAsyncEnumerable<TagDto> GetAll();
        Task<TagDto> AddAsync(TagDto tag);
        Task<TagDto> UpdateAsync(TagDto tag);
        Task DeleteAsync(Guid id);
    }
}
