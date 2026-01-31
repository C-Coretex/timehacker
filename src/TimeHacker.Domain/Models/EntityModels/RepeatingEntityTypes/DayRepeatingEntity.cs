namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

public class DayRepeatingEntity: IRepeatingEntityType
{
    private int _daysCountToRepeat;

    public int DaysCountToRepeat
    {
        get => _daysCountToRepeat;
        set
        {
            if (value < 1)
                throw new ArgumentException("Value must be at least 1", nameof(DaysCountToRepeat));

            _daysCountToRepeat = value;
        }
    }

    public DayRepeatingEntity()
    {}
    public DayRepeatingEntity(int daysCountToRepeat)
    {
        DaysCountToRepeat = daysCountToRepeat;
    }

    public DateOnly GetNextTaskDate(DateOnly startingFrom)
    {
        return startingFrom.AddDays(DaysCountToRepeat);
    }
}
