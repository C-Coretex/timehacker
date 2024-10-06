using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Contracts.Models.ReturnModels;

public class TaskContainerReturn
{
    public bool IsFixed { get; set; }
    public Guid? ScheduleEntityId { get; set; }
    public ITask Task { get; set; }
    public TimeRange TimeRange { get; set; }
}