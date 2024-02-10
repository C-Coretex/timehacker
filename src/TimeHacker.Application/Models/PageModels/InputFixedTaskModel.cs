namespace TimeHacker.Application.Models.PageModels
{
    public class InputFixedTaskModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public uint Priority { get; set; }
        public string StartTimestamp { get; set; }
        public string EndTimestamp { get; set; }
    }
}
