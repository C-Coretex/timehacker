namespace TimeHacker.Domain.Models.ReturnModels;

public record TasksForDayReturn
{
    public DateOnly Date { get; init; }
    public ICollection<TaskContainerReturn> TasksTimeline { get; init; } = [];
    public ICollection<CategoryContainerReturn> CategoriesTimeline { get; init; } = [];

    public static TasksForDayReturn Create(ScheduleSnapshot scheduleSnapshot)
    {
        ArgumentNullException.ThrowIfNull(scheduleSnapshot);

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

    /// <summary>
    /// Materializes this in-memory timeline into a persistable <see cref="ScheduleSnapshot"/> — the captured
    /// generated state of the day. Pass an existing snapshot to update it in place (the Clear()+repopulate
    /// supports the refresh path); pass null to create a new one.
    /// </summary>
    public ScheduleSnapshot CreateOrUpdateScheduleSnapshot(ScheduleSnapshot? entity = null)
    {
        var newEntity = entity ?? new ScheduleSnapshot();

        newEntity.Date = Date;

        // Remove existing entries and repopulate with the current timeline. This is a simple way to support replacing existing data with new.
        // Without Clear, the existing entries would not be removed.
        newEntity.ScheduledTasks.Clear();
        TasksTimeline.Select(x => x.CreateScheduledTask()).ForEach(newEntity.ScheduledTasks.Add);

        newEntity.ScheduledCategories.Clear();
        CategoriesTimeline.Select(x => x.CreateScheduledCategory()).ForEach(newEntity.ScheduledCategories.Add);

        return newEntity;
    }
}
