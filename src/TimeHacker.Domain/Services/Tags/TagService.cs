using TimeHacker.Domain.Contracts.Entities.Tags;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Tags;
using TimeHacker.Domain.Contracts.IServices.Tags;

namespace TimeHacker.Domain.Services.Tags
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly UserAccessorBase _userAccessorBase;

        public TagService(ITagRepository tagRepository, UserAccessorBase userAccessorBase)
        {
            _tagRepository = tagRepository;
            _userAccessorBase = userAccessorBase;
        }

        public IQueryable<Tag> GetAll()
        {
            var userId = _userAccessorBase.UserId!;
            return _tagRepository.GetAll().Where(x => x.UserId == userId);
        }

        public Task<Tag> AddAsync(Tag tag)
        {
            tag.UserId = _userAccessorBase.UserId!;
            return _tagRepository.AddAsync(tag);
        }

        public async Task<Tag> UpdateAsync(Tag tag)
        {
            var userId = _userAccessorBase.UserId;

            if (tag == null)
                throw new ArgumentException("Category must be valid");


            var oldTag = await _tagRepository.GetByIdAsync(tag.Id);
            if (oldTag == null)
                return await _tagRepository.AddAsync(tag);

            if (oldTag.UserId != userId)
                throw new ArgumentException("User can only edit its own categories.");

            tag.UserId = userId;
            return await _tagRepository.UpdateAsync(tag);
        }

        public async Task DeleteAsync(Guid id)
        {
            var userId = _userAccessorBase.UserId;
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                return;

            if (tag.UserId != userId)
                throw new ArgumentException("User can only delete its own categories.");

            await _tagRepository.DeleteAsync(tag);
        }
    }
}
