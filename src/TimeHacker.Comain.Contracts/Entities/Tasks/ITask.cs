using System.ComponentModel.DataAnnotations;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.Tasks
{
    public interface ITask : IDbModel<int>
    {
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
        public bool IsCompleted { get; set; }
        [Required]
        public DateTime CreatedTimestamp { get; set; }
    }
}
