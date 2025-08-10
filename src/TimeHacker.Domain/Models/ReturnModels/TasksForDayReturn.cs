using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Models.ReturnModels
{
    public record TasksForDayReturn
    {
        public DateOnly Date { get; init; }
        public List<TaskContainerReturn> TasksTimeline { get; init; } = [];
        public List<CategoryContainerReturn> CategoriesTimeline { get; init; } = [];

        public static TasksForDayReturn Create(ScheduleSnapshot scheduleSnapshot)
        {
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

        public ScheduleSnapshot CreateScheduleSnapshot(ScheduleSnapshot? entity = null)
        {
            entity ??= new ScheduleSnapshot();

            entity.Date = Date;
            entity.ScheduledTasks = TasksTimeline.Select(x => x.CreateScheduledTask()).ToList();
            entity.ScheduledCategories = CategoriesTimeline.Select(x => x.CreateScheduledCategory()).ToList();

            return entity;
        }
    }
}
