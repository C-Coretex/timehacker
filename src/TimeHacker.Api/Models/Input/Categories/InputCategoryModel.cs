using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Api.Models.Input.Categories;

public sealed record InputCategoryModel
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    public required string Name { get; init; }

    [StringLength(516)]
    public string? Description { get; init; }

    [Required]
    public required Color Color { get; init; }

    public CategoryDto CreateDto() =>
        new()
        {
            Name = Name,
            Description = Description,
            Color = Color
        };
}
