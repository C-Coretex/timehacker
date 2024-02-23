using Helpers.Domain.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TimeHacker.Domain.Models.Persistence.Categories;

namespace TimeHacker.Domain.Models.Persistence.Tasks
{
    [Index(nameof(UserId))]
    [Index(nameof(IsCompleted))]
    [Index(nameof(CreatedTimestamp))]
    [Index(nameof(StartTimestamp))]
    public class FixedTask : ITask
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
        public uint Priority { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        [Required]
        public DateTime StartTimestamp { get; set; }

        [Required]
        public DateTime EndTimestamp { get; set; }

        [Required]
        public DateTime CreatedTimestamp { get; set; } = DateTime.Now;

        [NotMapped]
        public List<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];
        [NotMapped]
        public List<Category> Categories { get; set; } = [];
    }
}
