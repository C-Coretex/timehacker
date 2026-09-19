using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Api.Models.Return.Categories;

public record CategoryReturnModel(
    Guid Id,
    string Name,
    string? Description,
    Color Color,
    IReadOnlyList<CategoryScheduleReturnModel> Schedules
)
{
    public static CategoryReturnModel Create(CategoryDto category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return new CategoryReturnModel(
            category.Id,
            category.Name,
            category.Description,
            category.Color,
            [.. category.Schedules.Select(CategoryScheduleReturnModel.Create)]);
    }
}
