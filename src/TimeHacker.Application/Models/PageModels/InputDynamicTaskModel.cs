using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Application.Models.PageModels
{
    public record InputDynamicTaskModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<int> CategoryIds { get; set; }
        public uint Priority { get; set; }
        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }
    }
}
