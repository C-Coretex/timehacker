using TimeHacker.Application.Api.Contracts.DTOs.Tags;
using TimeHacker.Application.Api.Contracts.IAppServices.Tags;
using TimeHacker.Domain.IRepositories.Tags;

namespace TimeHacker.Application.Api.AppServices.Tags;

public class TagService(ITagRepository tagRepository)
    : ITagAppService
{
    public IAsyncEnumerable<TagDto> GetAll()
    {
        return tagRepository.GetAll().Select(TagDto.Selector).AsAsyncEnumerable();
    }

    public async Task<TagDto> AddAsync(TagDto tag)
    {
        NotProvidedException.ThrowIfNull(tag);

        var entity = await tagRepository.AddAndSaveAsync(tag.GetEntity());
        return TagDto.Create(entity);
    }

    public async Task<TagDto> UpdateAsync(TagDto tag)
    {
        NotProvidedException.ThrowIfNull(tag);

        var entity = await tagRepository.GetByIdAsync(tag.Id!.Value);
        entity = await tagRepository.UpdateAndSaveAsync(tag.GetEntity(entity));
        return TagDto.Create(entity);
    }

    public Task DeleteAsync(Guid id)
    {
        return tagRepository.DeleteAndSaveAsync(id);
    }
}
