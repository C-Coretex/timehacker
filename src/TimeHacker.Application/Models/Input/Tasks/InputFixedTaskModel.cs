namespace TimeHacker.Application.Models.Input.Tasks
{
    public record InputFixedTaskModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<int> CategoryIds { get; set; }
        public uint Priority { get; set; }
        public string StartTimestamp { get; set; }
        public string EndTimestamp { get; set; }
    }
}
