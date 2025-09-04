using System.Drawing;
using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Api.Models.Return.Categories
{
    public record CategoryReturnModel(
        Guid Id,
        string Name,
        string? Description,
        Color Color
    )
    {
        public static CategoryReturnModel Create(CategoryDto category)
        {
            return new CategoryReturnModel(category.Id!.Value, category.Name, category.Description, category.Color);
        }
    }
}
