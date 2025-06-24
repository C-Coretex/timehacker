using System.Drawing;
using TimeHacker.Domain.Contracts.Entities.Categories;

namespace TimeHacker.Api.Models.Return.Categories
{
    public record CategoryReturnModel(
        Guid Id,
        string Name,
        string? Description,
        Color Color
    )
    {
        public static CategoryReturnModel Create(Category category)
        {
            return new CategoryReturnModel(category.Id, category.Name, category.Description, category.Color);
        }
    }
}
