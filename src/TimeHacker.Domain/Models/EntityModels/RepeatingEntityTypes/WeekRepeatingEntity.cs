using TimeHacker.Helpers.Domain.Extensions;

namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

public class WeekRepeatingEntity: IRepeatingEntityType
{
    public ICollection<Enums.DayOfWeek> RepeatsOn => field;

    public WeekRepeatingEntity(IEnumerable<Enums.DayOfWeek> repeatsOn)
    {
        var orderedRepeatsOn = repeatsOn.OrderBy(x => x).ToList();
        if (orderedRepeatsOn.Count == 0)
            throw new ArgumentException("At least one day of week must be chosen", nameof(repeatsOn));

        RepeatsOn = orderedRepeatsOn;
    }

    public DateOnly GetNextTaskDate(DateOnly startingFrom)
    {
        var currentDayOfWeek = (int)startingFrom.DayOfWeek;

        //Sunday is 7 in DayOfWeekEnum
        if (currentDayOfWeek == 0)
            currentDayOfWeek = 7;

        var nextDayOfWeek = (int?)RepeatsOn.FirstOrNull(x => (int)x > currentDayOfWeek);
        var daysToAdd = 0;
        if (!nextDayOfWeek.HasValue)
        {
            nextDayOfWeek = (int)RepeatsOn.First();
            //Add last day of week, since we need to travel from currentDayOfWeek to the end of the week
            daysToAdd += (int)Enums.DayOfWeek.Sunday;
        }

        daysToAdd += nextDayOfWeek.Value - currentDayOfWeek;

        return startingFrom.AddDays(daysToAdd);
    }
}
