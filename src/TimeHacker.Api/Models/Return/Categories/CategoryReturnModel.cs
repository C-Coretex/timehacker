using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Api.Models.Return.Categories;

public record CategoryReturnModel(
    Guid Id,
    string Name,
    string? Description,
    Color Color,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    ScheduleEntityReturnModel? ScheduleEntity
)
{
    public static CategoryReturnModel Create(CategoryDto category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return new CategoryReturnModel(
            category.Id!.Value,
            category.Name,
            category.Description,
            category.Color,
            category.Date,
            category.StartTime,
            category.EndTime,
            category.ScheduleEntity != null ? ScheduleEntityReturnModel.Create(category.ScheduleEntity) : null);
    }
}
