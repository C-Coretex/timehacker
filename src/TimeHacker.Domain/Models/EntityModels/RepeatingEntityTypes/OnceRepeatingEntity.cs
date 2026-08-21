using System.Text.Json.Serialization;

namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

/// <summary>
/// A recurrence whose occurrences are an explicit list of dates rather than a pattern — used for entities
/// that apply only on specific day(s). Unlike the pattern-based types, this series is finite.
/// </summary>
public class OnceRepeatingEntity : IRepeatingEntityType
{
    public ICollection<DateOnly> Dates { get; }

    public OnceRepeatingEntity(IEnumerable<DateOnly> dates)
    {
        ArgumentNullException.ThrowIfNull(dates);

        var orderedDates = dates.Distinct().OrderBy(x => x).ToList();
        if (orderedDates.Count == 0)
            throw new ArgumentException("At least one date must be chosen", nameof(dates));

        Dates = orderedDates;
    }

    // System.Text.Json binds this constructor when deserializing the polymorphic RepeatingEntity JSON from
    // the DB: its ICollection parameter matches the ICollection property (the IEnumerable overload's
    // parameter would not, which breaks System.Text.Json's parameterized-constructor binding).
    [JsonConstructor]
    public OnceRepeatingEntity(ICollection<DateOnly> dates)
        : this(dates.AsEnumerable())
    { }

    /// <returns>
    /// The next chosen date strictly after <paramref name="startingFrom"/>, or <c>null</c> once the list
    /// is exhausted.
    /// </returns>
    public DateOnly? GetNextTaskDate(DateOnly startingFrom)
    {
        // Projected to DateOnly? before FirstOrDefault: on the value type it would fall back to
        // 0001-01-01 instead of null.
        return Dates.Select(date => (DateOnly?)date).FirstOrDefault(date => date > startingFrom);
    }
}
