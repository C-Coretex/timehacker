using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Contracts.Models.ReturnModels;

public record TaskContainerReturn
{
    public bool IsFixed { get; init; }
    public Guid? ScheduleEntityId { get; init; }
    public ITask Task { get; init; }
    public TimeRange TimeRange { get; init; }

    public ScheduledTask CreateScheduledTask()
    {
        return new ScheduledTask()
        {
            Start = TimeRange.Start,
            End = TimeRange.End,
            IsFixed = IsFixed,
            UserId = Task.UserId,
            Name = Task.Name,
            Description = Task.Description,
            Priority = Task.Priority,
            ParentTaskId = Task.Id,
            ParentScheduleEntityId = ScheduleEntityId
        };
    }

    public static TaskContainerReturn Create(ScheduledTask task)
    {
        var entity = new TaskContainerReturn
        {
            IsFixed = task.IsFixed,
            ScheduleEntityId = task.ParentTaskId,
            TimeRange = new TimeRange(task.Start, task.End),
            Task = task.IsFixed ? 
                new FixedTask()
                {
                    Id = task.ParentTaskId,
                } : 
                new DynamicTask()
                {
                    Id = task.ParentTaskId,
                }
        };

        entity.Task.UserId = task.UserId;
        entity.Task.Name = task.Name;
        entity.Task.Description = task.Description;
        entity.Task.Priority = task.Priority;

        return entity;
    }
}