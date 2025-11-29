using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks
{
    public record CategoryContainerDto
    {
        public CategoryDto? Category { get; init; }
        public TimeRange TimeRange { get; init; }

        public static CategoryContainerDto Create(CategoryContainerReturn category)
        {
            return new CategoryContainerDto
            {
                Category = category.Category != null ? CategoryDto.Create(category.Category) : null,
                TimeRange = category.TimeRange
            };
        }
    }
}
