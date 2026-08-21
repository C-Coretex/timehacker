using System.Text.Json.Serialization;

namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

[JsonDerivedType(typeof(DayRepeatingEntity), "day")]
[JsonDerivedType(typeof(WeekRepeatingEntity), "week")]
[JsonDerivedType(typeof(MonthRepeatingEntity), "month")]
[JsonDerivedType(typeof(YearRepeatingEntity), "year")]
[JsonDerivedType(typeof(OnceRepeatingEntity), "once")]
public interface IRepeatingEntityType
{
    DateOnly? GetNextTaskDate(DateOnly startingFrom);
}
