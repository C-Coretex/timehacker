using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks;

public record CategoryContainerDto
{
    public Guid? ScheduleEntityId { get; init; }
    public required CategoryDto Category { get; init; }
    public TimeRange TimeRange { get; init; }

    public static CategoryContainerDto Create(CategoryContainerReturn category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return new CategoryContainerDto
        {
            ScheduleEntityId = category.ScheduleEntityId,
            Category = CategoryDto.Create(category.Category),
            TimeRange = category.TimeRange
        };
    }
}
