using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Domain.BusinessLogicExceptions;

namespace TimeHacker.Api.Models.Input.Tasks;

public record InputDynamicTaskModel
{
    [Required]
    [StringLength(250, MinimumLength = 1)]
    public required string Name { get; init; }

    [StringLength(516)]
    public string? Description { get; init; }

    public IEnumerable<Guid> CategoryIds { get; init; } = [];

    [Required] public required byte Priority { get; init; }

    [Required] public required TimeSpan MinTimeToFinish { get; init; }

    [Required] public required TimeSpan MaxTimeToFinish { get; init; }

    public TimeSpan? OptimalTimeToFinish { get; init; }

    public DynamicTaskDto CreateDto()
    {
        if (MinTimeToFinish >= MaxTimeToFinish)
            throw new DataIsNotCorrectException($"{nameof(MinTimeToFinish)} must be less than {nameof(MaxTimeToFinish)}.", nameof(MinTimeToFinish));

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
