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
        ArgumentNullException.ThrowIfNull(scheduleEntity);
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

    /// <summary>
    /// Expands the recurrence pattern into the concrete occurrence dates that fall within [from, to].
    /// Starting point depends on intent: if the range overlaps already-generated dates
    /// (<see cref="LastEntityCreated"/> > from, i.e. a refresh/recalculation) we replay from
    /// <see cref="FirstEntityCreated"/>; otherwise we continue forward from the last generated date.
    /// </summary>
    public IEnumerable<DateOnly> GetNextEntityDatesIn(DateOnly from, DateOnly to)
    {
        // Hard cap so a malformed pattern that never advances past EndsOn can't loop forever.
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

    /// <returns>
    /// Whether <paramref name="date"/> is an actual occurrence of this recurrence. There is no
    /// closed-form check, so it regenerates the series from creation until it reaches or passes the target
    /// — O(number of occurrences before <paramref name="date"/>). Used to validate generated dates.
    /// </returns>
    public bool IsEntityDateCorrect(DateOnly date)
    {
        if (date == FirstEntityCreated || date == LastEntityCreated)
            return true;

        var maxIterations = 10_000;
        var nextTaskDate = LastEntityCreated == null || date <= LastEntityCreated ? DateOnly.FromDateTime(CreatedTimestamp) : LastEntityCreated.Value;

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
