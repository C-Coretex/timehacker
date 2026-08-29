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

    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }

    public ScheduleEntityDto? ScheduleEntity { get; init; }

    public static Expression<Func<Category, CategoryDto>> Selector =>
        category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            Date = category.Date,
            StartTime = category.StartTime,
            EndTime = category.EndTime,
            ScheduleEntity = category.ScheduleEntity != null ? new ScheduleEntityDto(
                category.ScheduleEntity.Id,
                category.ScheduleEntity.RepeatingEntity,
                category.ScheduleEntity.CreatedTimestamp,
                category.ScheduleEntity.LastEntityCreated,
                category.ScheduleEntity.EndsOn
            ) : null
        };

    private static readonly Func<Category, CategoryDto> CreateFunc = Selector.Compile();
    public static CategoryDto Create(Category category) => CreateFunc(category);

    //TODO: should it assign its navigation property (ScheduledEntity)?
    public Category GetEntity(Category? category = null)
    {
        category ??= new Category();

        category.Name = Name;
        category.Description = Description;
        category.Color = Color;
        category.Date = Date;
        category.StartTime = StartTime;
        category.EndTime = EndTime;

        return category;
    }
}
