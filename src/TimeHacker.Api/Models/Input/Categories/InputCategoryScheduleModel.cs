using TimeHacker.Application.Api.Contracts.DTOs.Categories;

namespace TimeHacker.Api.Models.Input.Categories;

public sealed record InputCategoryScheduleModel
{
    /// <summary>Optional note distinguishing this window from the category's others.</summary>
    [StringLength(516)]
    public string? Description { get; init; }

    /// <summary>The day this window applies to; a schedule, if attached later, repeats it after that day.</summary>
    [Required]
    public required DateOnly Date { get; init; }

    [Required]
    public required TimeOnly StartTime { get; init; }

    [Required]
    public required TimeOnly EndTime { get; init; }

    public CategoryScheduleDto CreateDto(Guid categoryId)
    {
        ValidateWindow();

        return new CategoryScheduleDto
        {
            CategoryId = categoryId,
            Description = Description,
            Date = Date,
            StartTime = StartTime,
            EndTime = EndTime
        };
    }

    // Wall-clock times, never UTC-converted, so an overnight window would invert rather than wrap.
    // Kept parameterless so the reported name stays the client-facing property, not a method argument.
    private void ValidateWindow()
    {
        if (StartTime >= EndTime)
            throw new DataIsNotCorrectException($"{nameof(StartTime)} must be before {nameof(EndTime)}.", nameof(StartTime));
    }
}
