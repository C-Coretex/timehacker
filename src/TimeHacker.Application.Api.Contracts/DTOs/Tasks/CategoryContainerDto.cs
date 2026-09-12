using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks;

public record CategoryContainerDto
{
    public Guid? ScheduleEntityId { get; init; }
    public Guid CategoryScheduleId { get; init; }

    /// <summary>The window's own optional note; the band is labelled by the category's name.</summary>
    public string? ScheduleDescription { get; init; }

    /// <summary>The parent category, which is what tasks are linked to.</summary>
    public required CategoryDto Category { get; init; }

    public TimeRange TimeRange { get; init; }

    public static CategoryContainerDto Create(CategoryContainerReturn category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return new CategoryContainerDto
        {
            ScheduleEntityId = category.ScheduleEntityId,
            CategoryScheduleId = category.CategoryScheduleId,
            ScheduleDescription = category.ScheduleDescription,
            Category = CategoryDto.Create(category.Category),
            TimeRange = category.TimeRange
        };
    }
}
