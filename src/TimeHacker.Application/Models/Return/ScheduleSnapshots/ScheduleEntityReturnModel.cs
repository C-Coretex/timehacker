using TimeHacker.Domain.Contracts.Models.EntityModels;

namespace TimeHacker.Application.Models.Return.ScheduleSnapshots
{
    public class ScheduleEntityReturnModel
    {
        public uint Id { get; init; }

        public RepeatingEntityModel RepeatingEntity { get; set; }
        public DateTime ScheduleCreated { get; set; } = DateTime.UtcNow;
        public DateTime LastTaskCreated { get; set; }
        public DateOnly? EndsOn { get; set; }
    }
}
