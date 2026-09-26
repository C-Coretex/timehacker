using TimeHacker.Application.Api.Contracts.DTOs.Categories;

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
    [Range(PriorityConstants.Highest, PriorityConstants.Lowest)]
    public required byte Priority { get; init; }

    [Required]
    public required DateTime StartTimestamp { get; init; }

    [Required]
    public required DateTime EndTimestamp { get; init; }

    public FixedTaskDto CreateDto()
    {
        if (StartTimestamp >= EndTimestamp)
            throw new DataIsNotCorrectException($"{nameof(StartTimestamp)} must be before {nameof(EndTimestamp)}.", nameof(StartTimestamp));

        return new FixedTaskDto
        {
            Name = Name,
            Description = Description,
            Priority = Priority,
            StartTimestamp = StartTimestamp,
            EndTimestamp = EndTimestamp,
            Categories = [.. CategoryIds.Select(LinkCategoryDto.EmptyLink)]
        };
    }
}
