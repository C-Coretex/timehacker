using Helpers.Domain.Abstractions.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Domain.Models.Tasks
{
    public interface ITask : IModel
    {
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
        public bool IsCompleted { get; set; }
        [Required]
        public DateTime CreatedTimestamp { get; set; }
    }
}
