using System.Drawing;
using System.ComponentModel.DataAnnotations;
using TimeHacker.Domain.Entities.Categories;

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
