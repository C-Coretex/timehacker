namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

public class MonthRepeatingEntity: IRepeatingEntityType
{
    private const byte MaxMonthDay = 31;

    private byte _monthDayToRepeat;
    public byte MonthDayToRepeat
    {
        get => _monthDayToRepeat;
        set
        {
            _monthDayToRepeat = value switch
            {
                > MaxMonthDay => throw new ArgumentException(
                    $"Property value must not be greater than maximum days in month ({MaxMonthDay})",
                    nameof(MonthDayToRepeat)),
                < 1 => throw new ArgumentException($"Property value must be at least first day in month (1)",
                    nameof(MonthDayToRepeat)),
                _ => value
            };
        }
    }

    public MonthRepeatingEntity()
    {}
    public MonthRepeatingEntity(byte monthDayToRepeat)
    {
        MonthDayToRepeat = monthDayToRepeat;
    }

    /// <returns>
    /// The next occurrence of <see cref="MonthDayToRepeat"/> strictly after <paramref name="startingFrom"/>.
    /// Months vary in length, so a target like day 31 doesn't exist every month: the loop skips forward month
    /// by month (at most a year) until it finds one long enough to contain the target day.
    /// </returns>
    public DateOnly? GetNextTaskDate(DateOnly startingFrom)
    {
        const int maxIterations = 12;
        var startingDay = startingFrom.Day;

        //Reset to 1-st day
        startingFrom = startingFrom.AddDays(-startingFrom.Day + 1);
        // If this month's target day is already today-or-past, the next occurrence is in a later month.
        if (startingDay >= MonthDayToRepeat)
            startingFrom = startingFrom.AddMonths(1);

        for (var i = 0; i < maxIterations; i++)
        {
            var maxDayInMonth = DateTime.DaysInMonth(startingFrom.Year, startingFrom.Month);
            if (maxDayInMonth >= MonthDayToRepeat)
                return startingFrom.AddDays(MonthDayToRepeat - 1);

            // Month too short for the target day (e.g. day 31 in a 30-day month) — try the next one.
            startingFrom = startingFrom.AddMonths(1);
        }

        throw new InvalidOperationException("No next task date found");
    }
}
