using Microsoft.EntityFrameworkCore;
using TimeHacker.Application.Api.Contracts.IAppServices.Tags;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.IRepositories.Tags;

namespace TimeHacker.Application.Api.AppServices.Tags
{
    public class TagService(ITagRepository tagRepository)
        : ITagAppService
    {
        public IAsyncEnumerable<Tag> GetAll()
        {
            return tagRepository.GetAll().AsAsyncEnumerable();
        }

        public Task<Tag> AddAsync(Tag tag)
        {
            return tagRepository.AddAndSaveAsync(tag);
        }

        public Task<Tag> UpdateAsync(Tag tag)
        {
            if (tag == null)
                throw new NotProvidedException(nameof(tag));

            return tagRepository.UpdateAndSaveAsync(tag);
        }

        public Task DeleteAsync(Guid id)
        {

            return tagRepository.DeleteAndSaveAsync(id);
        }
    }
}
