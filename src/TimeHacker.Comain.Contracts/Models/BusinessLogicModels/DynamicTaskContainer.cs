using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Domain.Contracts.Models.BusinessLogicModels
{
    public struct DynamicTaskContainer
    {
        public DynamicTask Task { get; init; }
        public int CountOfUses { get; init; }
        public TimeRange TimeRange { get; set; } = new TimeRange();

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
}
