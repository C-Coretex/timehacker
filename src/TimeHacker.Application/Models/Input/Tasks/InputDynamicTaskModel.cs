using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Application.Models.Input.Tasks
{
    public record InputDynamicTaskModel
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<Guid> CategoryIds { get; set; }
        [Required]
        public byte Priority { get; set; }
        [Required]
        public TimeSpan MinTimeToFinish { get; set; }
        [Required]
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }
    }
}
