namespace TimeHacker.Application.Models.Return.ScheduleSnapshots
{
    public class ScheduledTaskReturnModel
    {
        public ulong Id { get; init; }
        public uint ParentTaskId { get; init; }

        public DateOnly Date { get; set; }

        public bool IsFixed { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public uint Priority { get; set; }
        public bool IsCompleted { get; set; } = false;
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
    }
}
