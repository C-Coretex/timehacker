namespace TimeHacker.Application.Models.Return.Tasks
{
    public class FixedTaskReturnModel
    {
        public uint Id { get; init; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public uint Priority { get; set; }

        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
