using System.Drawing;
using TimeHacker.Domain.Contracts.Entities.Tags;

namespace TimeHacker.Api.Models.Return.Tags
{
    public record TagReturnModel(
        Guid Id,
        string Name,
        string? Category,
        Color Color
    )
    {
        public static TagReturnModel Create(Tag tag)
        {
            return new TagReturnModel(tag.Id, tag.Name, tag.Category, tag.Color);
        }
    }
}
