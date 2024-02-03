using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Domain.Models.Tasks
{
    [Index(nameof(UserId))]
    [Index(nameof(Category))]
    [Index(nameof(IsCompleted))]
    [Index(nameof(CreatedTimestamp))]
    public class DynamicTask : ITask
    {
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public uint Priority { get; set; }

        [Required]
        public TimeSpan MinTimeToFinish { get; set; }

        [Required]
        public TimeSpan MaxTimeToFinish { get; set; }

        public TimeSpan? OptimalTimeToFinish { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        [Required]
        public DateTime CreatedTimestamp { get; set; } = DateTime.Now;
    }
}
