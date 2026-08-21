namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

public class YearRepeatingEntity: IRepeatingEntityType
{
    private const int MaxYearDay = 366;

    private int _yearDayToRepeat;
    public int YearDayToRepeat
    {
        get => _yearDayToRepeat;
        set
        {
            _yearDayToRepeat = value switch
            {
                > MaxYearDay => throw new ArgumentException(
                    $"Property value must not be greater than maximum days in year ({MaxYearDay})",
                    nameof(YearDayToRepeat)),
                < 1 => throw new ArgumentException(
                    $"Property value must be at least first day in year (1)",
                    nameof(YearDayToRepeat)),
                _ => value
            };
        }
    }

    public YearRepeatingEntity()
    {}

    public YearRepeatingEntity(int yearDayToRepeat)
    {
        YearDayToRepeat = yearDayToRepeat;
    }

    /// <returns>
    /// The next occurrence of day-of-year <see cref="YearDayToRepeat"/> strictly after
    /// <paramref name="startingFrom"/>. Day 366 only exists in leap years, so the loop skips non-leap years
    /// (at most 4, the leap cycle) until it finds one long enough to contain the target day-of-year.
    /// </returns>
    public DateOnly? GetNextTaskDate(DateOnly startingFrom)
    {
        const int maxIterations = 4;
        var startingDay = startingFrom.DayOfYear;
        // Reset to Jan 1 of the current year (zero out day then month).
        startingFrom = startingFrom.AddDays(-startingFrom.Day + 1);
        startingFrom = startingFrom.AddMonths(-startingFrom.Month + 1);

        // If this year's target day is already today-or-past, the next occurrence is in a later year.
        if (startingDay >= YearDayToRepeat)
            startingFrom = startingFrom.AddYears(1);

        for (var i = 0; i < maxIterations; i++)
        {
            var maxDayInYear = DateTime.IsLeapYear(startingFrom.Year) ? MaxYearDay : MaxYearDay - 1;
            if (maxDayInYear >= YearDayToRepeat)
                return startingFrom.AddDays(YearDayToRepeat - 1);

            // Year too short for the target day (day 366 in a non-leap year) — try the next one.
            startingFrom = startingFrom.AddYears(1);
        }

        throw new InvalidOperationException("No next task date found");
    }
}
