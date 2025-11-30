namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks;

public record TasksForDayDto
{
    public DateOnly Date { get; init; }
    public IEnumerable<TaskContainerDto> TasksTimeline { get; init; } = [];
    public IEnumerable<CategoryContainerDto> CategoriesTimeline { get; init; } = [];

    public static TasksForDayDto Create(TasksForDayReturn tasksForDay)
    {
        return new TasksForDayDto
        {
            Date = tasksForDay.Date,
            TasksTimeline = tasksForDay.TasksTimeline
                .Select(TaskContainerDto.Create),
            CategoriesTimeline = tasksForDay.CategoriesTimeline
                .Select(CategoryContainerDto.Create)
        };
    }
}
