using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Api.Models.Input.Tasks;

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

    public FixedTaskDto CreateFixedTaskDto()
    {
        return new FixedTaskDto
        {
            Name = Name,
            Description = Description,
            Priority = Priority,
            StartTimestamp = DateTime.Parse(StartTimestamp),
            EndTimestamp = DateTime.Parse(EndTimestamp)
        };
    }
}
