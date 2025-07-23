using TimeHacker.Domain.Entities.Tags;

namespace TimeHacker.Domain.IServices.Tags
{
    public interface ITagService
    {
        IQueryable<Tag> GetAll();
        Task<Tag> AddAsync(Tag tag);
        Task<Tag> UpdateAsync(Tag tag);
        Task DeleteAsync(Guid id);
    }
}
