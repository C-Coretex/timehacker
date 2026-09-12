using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Api.Models.Return.Categories;

public record CategoryScheduleReturnModel(
    Guid Id,
    Guid CategoryId,
    string? Description,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    ScheduleEntityReturnModel? ScheduleEntity
)
{
    public static CategoryScheduleReturnModel Create(CategoryScheduleDto schedule)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        return new CategoryScheduleReturnModel(
            schedule.Id!.Value,
            schedule.CategoryId,
            schedule.Description,
            schedule.Date,
            schedule.StartTime,
            schedule.EndTime,
            schedule.ScheduleEntity != null ? ScheduleEntityReturnModel.Create(schedule.ScheduleEntity) : null);
    }
}
