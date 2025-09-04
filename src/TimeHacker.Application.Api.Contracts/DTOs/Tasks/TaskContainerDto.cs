using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.Models.BusinessLogicModels;
using TimeHacker.Domain.Models.ReturnModels;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks
{
    public record TaskContainerDto
    {
        public bool IsFixed { get; init; }
        public Guid? ScheduleEntityId { get; init; }
        public ITask Task { get; init; }
        public TimeRange TimeRange { get; init; }

        public static TaskContainerDto Create(TaskContainerReturn task)
        {
            return new TaskContainerDto
            {
                IsFixed = task.IsFixed,
                ScheduleEntityId = task.ScheduleEntityId,
                TimeRange = task.TimeRange,
                Task = task.Task
            };
        }
    }
}
