using System.Drawing;
using TimeHacker.Domain.Contracts.Entities.Categories;
using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Api.Models.Input.Categories
{
    public record InputCategoryModel
    {
        [Required]
        public required string Name { get; init; }
        
        public string? Description { get; init; }

        [Required]
        public required Color Color { get; init; }


        public Category CreateCategory()
        {
            return new Category()
            {
                Name = Name,
                Description = Description,
                Color = Color
            };
        }
    }
}
