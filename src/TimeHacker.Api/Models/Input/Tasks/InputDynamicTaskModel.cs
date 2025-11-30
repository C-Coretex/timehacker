using TimeHacker.Application.Api.Contracts.DTOs.Tasks;

namespace TimeHacker.Api.Models.Input.Tasks;

public record InputDynamicTaskModel
{
    [Required] public required string Name { get; init; }
    public string? Description { get; init; }

    public IEnumerable<Guid> CategoryIds { get; init; } = [];

    [Required] public required byte Priority { get; init; }

    [Required] public required TimeSpan MinTimeToFinish { get; init; }

    [Required] public required TimeSpan MaxTimeToFinish { get; init; }

    public TimeSpan? OptimalTimeToFinish { get; init; }

    public DynamicTaskDto CreateDynamicTaskDto()
    {
        return new DynamicTaskDto()
        {
            Name = Name,
            Description = Description,
            Priority = Priority,
            MinTimeToFinish = MinTimeToFinish,
            MaxTimeToFinish = MaxTimeToFinish,
            OptimalTimeToFinish = OptimalTimeToFinish
        };
    }
}
