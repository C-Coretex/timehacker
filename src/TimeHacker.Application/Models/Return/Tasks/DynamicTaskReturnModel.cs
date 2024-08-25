namespace TimeHacker.Application.Models.Return.Tasks
{
    public class DynamicTaskReturnModel
    {
        public uint Id { get; init; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public uint Priority { get; set; }

        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }
}
