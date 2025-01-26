namespace TimeHacker.Application.Models.Input.Tasks
{
    public record InputDynamicTaskModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<Guid> CategoryIds { get; set; }
        public byte Priority { get; set; }
        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }
    }
}
