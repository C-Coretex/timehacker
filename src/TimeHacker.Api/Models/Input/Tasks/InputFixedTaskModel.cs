using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
using TimeHacker.Domain.BusinessLogicExceptions;

namespace TimeHacker.Api.Models.Input.Tasks;

public record InputFixedTaskModel
{
    [Required]
    [StringLength(250, MinimumLength = 1)]
    public required string Name { get; init; }

    [StringLength(516)]
    public string? Description { get; init; }

    public IEnumerable<Guid> CategoryIds { get; init; } = [];

    [Required]
    public required byte Priority { get; init; }

    [Required]
    public required string StartTimestamp { get; init; }

    [Required]
    public required string EndTimestamp { get; init; }

    public FixedTaskDto CreateDto()
    {
        if (!DateTime.TryParse(StartTimestamp, out var start))
            throw new DataIsNotCorrectException($"'{StartTimestamp}' is not a valid date-time.", nameof(StartTimestamp));
        if (!DateTime.TryParse(EndTimestamp, out var end))
            throw new DataIsNotCorrectException($"'{EndTimestamp}' is not a valid date-time.", nameof(EndTimestamp));
        if (start >= end)
            throw new DataIsNotCorrectException($"{nameof(StartTimestamp)} must be before {nameof(EndTimestamp)}.", nameof(StartTimestamp));

        return new FixedTaskDto
        {
            Name = Name,
            Description = Description,
            Priority = Priority,
            StartTimestamp = start,
            EndTimestamp = end
        };
    }
}
