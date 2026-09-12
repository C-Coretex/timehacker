using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Application.Api.Contracts.DTOs.Categories;

public record CategoryScheduleDto
{
    public Guid? Id { get; init; }
    public Guid CategoryId { get; init; }

    /// <summary>Optional note distinguishing this window from the category's others.</summary>
    public string? Description { get; init; }

    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }

    public ScheduleEntityDto? ScheduleEntity { get; init; }

    public static Expression<Func<CategorySchedule, CategoryScheduleDto>> Selector =>
        schedule => new CategoryScheduleDto
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
        };

    private static readonly Func<CategorySchedule, CategoryScheduleDto> CreateFunc = Selector.Compile();
    public static CategoryScheduleDto Create(CategorySchedule schedule) => CreateFunc(schedule);

    /// <summary>
    /// Writes the window's own fields. The recurrence link is owned solely by ScheduleEntityAppService, so
    /// <see cref="ScheduleEntity"/> is read-only here and deliberately never written back.
    /// </summary>
    public CategorySchedule GetEntity(CategorySchedule? schedule = null)
    {
        schedule ??= new CategorySchedule { CategoryId = CategoryId };

        schedule.Description = Description;
        schedule.Date = Date;
        schedule.StartTime = StartTime;
        schedule.EndTime = EndTime;

        return schedule;
    }
}
