using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.Models.ReturnModels;

public record ScheduleEntityReturn
{
    public Guid Id { get; init; }
    public DateTime CreatedTimestamp { get; init; }

    public Guid UserId { get; init; }

    public RepeatingEntityDto RepeatingEntity { get; init; } = null!;
    public DateOnly? FirstEntityCreated { get; init; }
    public DateOnly? LastEntityCreated { get; init; }
    public DateOnly? EndsOn { get; init; }

    public virtual ICollection<ScheduledTask> ScheduledTasks { get; init; } = [];
    public virtual ICollection<ScheduledCategory> ScheduledCategories { get; init; } = [];

    public virtual FixedTask? FixedTask { get; init; }
    public virtual Category? Category { get; init; }

    public static ScheduleEntityReturn Create(ScheduleEntity scheduleEntity)
    {
        ArgumentNullException.ThrowIfNull(scheduleEntity, nameof(scheduleEntity));
        return new ScheduleEntityReturn()
        {
            Id = scheduleEntity.Id,
            UserId = scheduleEntity.UserId,
            RepeatingEntity = scheduleEntity.RepeatingEntity,
            FirstEntityCreated = scheduleEntity.FirstEntityCreated,
            CreatedTimestamp = scheduleEntity.CreatedTimestamp,
            LastEntityCreated = scheduleEntity.LastEntityCreated,
            EndsOn = scheduleEntity.EndsOn,
            ScheduledTasks = scheduleEntity.ScheduledTasks,
            ScheduledCategories = scheduleEntity.ScheduledCategories,
            FixedTask = scheduleEntity.FixedTask,
            Category = scheduleEntity.Category
        };
    }

    public IEnumerable<DateOnly> GetNextEntityDatesIn(DateOnly from, DateOnly to)
    {
        var maxIterations = 10_000;
        //if we are recalculating already calculated data - go from beginning
        var nextTaskDate = (LastEntityCreated > from ? FirstEntityCreated : LastEntityCreated) ?? DateOnly.FromDateTime(CreatedTimestamp); 
        
        while (nextTaskDate < to)
        {
            nextTaskDate = RepeatingEntity.RepeatingData.GetNextTaskDate(nextTaskDate);
            if (nextTaskDate > EndsOn || maxIterations-- == 0)
                yield break;

            if (nextTaskDate >= from && nextTaskDate <= to)
                yield return nextTaskDate;
        }
    }

    public bool IsEntityDateCorrect(DateOnly date)
    {
        var maxIterations = 10_000;
        var nextTaskDate = DateOnly.FromDateTime(CreatedTimestamp);

        while (nextTaskDate <= date)
        {
            nextTaskDate = RepeatingEntity.RepeatingData.GetNextTaskDate(nextTaskDate);
            if (nextTaskDate > EndsOn || maxIterations-- == 0)
                return false;

            if (nextTaskDate == date)
                return true;
        }

        return false;
    }
}
