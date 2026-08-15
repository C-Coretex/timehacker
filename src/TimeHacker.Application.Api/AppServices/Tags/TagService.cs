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

        var entity = await tagRepository.GetAndUpdateAndSaveAsync(tag.Id!.Value, t => tag.GetEntity(t));
        if (entity is null)
            throw new NotFoundException("Tag", tag.Id!.Value.ToString());

        return TagDto.Create(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!await tagRepository.DeleteAndSaveAsync(id))
            throw new NotFoundException("Tag", id.ToString());
    }
}
