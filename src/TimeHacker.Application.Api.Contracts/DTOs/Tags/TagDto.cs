using System.Drawing;
using TimeHacker.Domain.Entities.Tags;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tags
{
    public record TagDto(
        Guid? Id,
        string Name,
        string? Category,
        Color Color)
    {
        public static Expression<Func<Tag, TagDto>> Selector =>
            x => new TagDto(
                x.Id,
                x.Name,
                x.Category,
                x.Color);

        private static readonly Func<Tag, TagDto> CreateFunc = Selector.Compile();
        public static TagDto Create(Tag entity) => CreateFunc(entity);

        public Tag GetEntity(Tag? entity = null)
        {
            entity ??= new Tag()
            {
                Id = Id ?? Guid.CreateVersion7()
            };

            entity.Name = Name;
            entity.Category = Category;
            entity.Color = Color;

            return entity;
        }
    }
}
