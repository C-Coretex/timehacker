using System.ComponentModel.DataAnnotations;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Api.Models.Input.Tasks
{
    public record InputFixedTaskModel
    {
        [Required] 
        public required string Name { get; init; }
        public string? Description { get; init; }

        public IEnumerable<Guid> CategoryIds { get; init; } = [];

        [Required] 
        public required byte Priority { get; init; }

        [Required] 
        public required string StartTimestamp { get; init; }

        [Required] 
        public required string EndTimestamp { get; init; }

        public FixedTask CreateFixedTask()
        {
            return new FixedTask
            {
                Name = Name,
                Description = Description,
                Priority = Priority,
                StartTimestamp = DateTime.Parse(StartTimestamp),
                EndTimestamp = DateTime.Parse(EndTimestamp)
            };
        }
    }
}
