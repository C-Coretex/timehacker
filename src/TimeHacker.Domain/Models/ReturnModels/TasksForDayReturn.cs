using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Models.ReturnModels;

public record TasksForDayReturn
{
    public DateOnly Date { get; init; }
    public ICollection<TaskContainerReturn> TasksTimeline { get; init; } = [];
    public ICollection<CategoryContainerReturn> CategoriesTimeline { get; init; } = [];

    public static TasksForDayReturn Create(ScheduleSnapshot scheduleSnapshot)
    {
        ArgumentNullException.ThrowIfNull(scheduleSnapshot, nameof(scheduleSnapshot));

        return new TasksForDayReturn()
        {
            Date = scheduleSnapshot.Date,
            TasksTimeline = scheduleSnapshot.ScheduledTasks
                .Select(TaskContainerReturn.Create)
                .ToList(),
            CategoriesTimeline = scheduleSnapshot.ScheduledCategories
                .Select(CategoryContainerReturn.Create)
                .ToList()
        };
    }

    public ScheduleSnapshot CreateOrUpdateScheduleSnapshot(ScheduleSnapshot? entity = null)
    {
        var newEntity = entity ?? new ScheduleSnapshot();

        newEntity.Date = Date;

        newEntity.ScheduledTasks.Clear();
        TasksTimeline.Select(x => x.CreateScheduledTask()).ForEach(newEntity.ScheduledTasks.Add);

        newEntity.ScheduledCategories.Clear();
        CategoriesTimeline.Select(x => x.CreateScheduledCategory()).ForEach(newEntity.ScheduledCategories.Add);

        return newEntity;
    }
}
