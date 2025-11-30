using System.Drawing;
using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Api.Models.Input.Categories;

public record InputCategoryModel
{
    public Guid? ScheduleEntityId { get; set; }

    [Required]
    public required string Name { get; init; }
    
    public string? Description { get; init; }

    [Required]
    public required Color Color { get; init; }


    public CategoryDto CreateCategory()
    {
        return new CategoryDto(ScheduleEntityId, Name, Description, Color)
        {
        };
    }
}
