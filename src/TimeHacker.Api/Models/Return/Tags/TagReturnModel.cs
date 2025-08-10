using System.Drawing;
using TimeHacker.Application.Api.Contracts.DTOs.Tags;

namespace TimeHacker.Api.Models.Return.Tags
{
    public record TagReturnModel(
        Guid Id,
        string Name,
        string? Category,
        Color Color
    )
    {
        public static TagReturnModel Create(TagDto tag) =>  new TagReturnModel(tag.Id!.Value, tag.Name, tag.Category, tag.Color);
    }
}
