using System.Text.Json.Serialization;

namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

public class WeekRepeatingEntity: IRepeatingEntityType
{
    public ICollection<Enums.DayOfWeek> RepeatsOn { get; }

    public WeekRepeatingEntity(IEnumerable<Enums.DayOfWeek> repeatsOn)
    {
        var orderedRepeatsOn = repeatsOn.OrderBy(x => x).ToList();
        if (orderedRepeatsOn.Count == 0)
            throw new ArgumentException("At least one day of week must be chosen", nameof(repeatsOn));

        RepeatsOn = orderedRepeatsOn;
    }

    // System.Text.Json binds this constructor when deserializing the polymorphic RepeatingEntity JSON from
    // the DB: its ICollection parameter matches the ICollection property (the IEnumerable overload's
    // parameter would not, which breaks System.Text.Json's parameterized-constructor binding).
    [JsonConstructor]
    public WeekRepeatingEntity(ICollection<Enums.DayOfWeek> repeatsOn)
        :this(repeatsOn.AsEnumerable())
    { }

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
