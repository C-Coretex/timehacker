using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Domain.Models.BusinessLogicModels;

public readonly record struct DynamicTaskContainer
{
    public DynamicTask Task { get; init; }
    public int CountOfUses { get; init; }
    public TimeRange TimeRange { get; init; } = new TimeRange();

    public DynamicTaskContainer(DynamicTask task, int countOfUses)
    {
        Task = task;
        CountOfUses = countOfUses;
    }
    public DynamicTaskContainer(DynamicTask task)
    {
        Task = task;
        CountOfUses = 0;
    }
}
