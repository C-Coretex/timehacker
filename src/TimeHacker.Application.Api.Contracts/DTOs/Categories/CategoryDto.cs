using System.Drawing;
using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Application.Api.Contracts.DTOs.Categories;

public record CategoryDto
{
    public Guid? Id { get; init; }

    public required string Name { get; init; }
    public string? Description { get; init; }
    public Color Color { get; init; }

    /// <summary>The dated time windows this category occupies. Managed through ICategoryScheduleAppService.</summary>
    public IReadOnlyCollection<CategoryScheduleDto> Schedules { get; init; } = [];

    // Schedules are projected inline rather than through CategoryScheduleDto.Selector so the whole
    // graph stays a single EF-translatable expression.
    public static Expression<Func<Category, CategoryDto>> Selector =>
        category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            Schedules = category.Schedules.Select(schedule => new CategoryScheduleDto
            {
                Id = schedule.Id,
                CategoryId = schedule.CategoryId,
                Description = schedule.Description,
                Date = schedule.Date,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                ScheduleEntity = schedule.ScheduleEntity != null ? new ScheduleEntityDto(
                    schedule.ScheduleEntity.Id,
                    schedule.ScheduleEntity.RepeatingEntity,
                    schedule.ScheduleEntity.CreatedTimestamp,
                    schedule.ScheduleEntity.LastEntityCreated,
                    schedule.ScheduleEntity.EndsOn
                ) : null
            }).ToList()
        };

    private static readonly Func<Category, CategoryDto> CreateFunc = Selector.Compile();
    public static CategoryDto Create(Category category) => CreateFunc(category);

    /// <summary>
    /// Writes the category's own fields only. <see cref="Schedules"/> is a read projection — schedules are
    /// created, updated and deleted through their own endpoints, never as a side effect of saving a category.
    /// </summary>
    public Category GetEntity(Category? category = null)
    {
        category ??= new Category();

        category.Name = Name;
        category.Description = Description;
        category.Color = Color;

        return category;
    }
}
