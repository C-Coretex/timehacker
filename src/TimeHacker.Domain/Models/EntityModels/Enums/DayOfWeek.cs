namespace TimeHacker.Domain.Models.EntityModels.Enums;

public enum DayOfWeek
{
    None = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

public static class DayOfWeekExtensions
{
    public static DayOfWeek ToDayOfWeek(this System.DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        System.DayOfWeek.Monday => DayOfWeek.Monday,
        System.DayOfWeek.Tuesday => DayOfWeek.Tuesday,
        System.DayOfWeek.Wednesday => DayOfWeek.Wednesday,
        System.DayOfWeek.Thursday => DayOfWeek.Thursday,
        System.DayOfWeek.Friday => DayOfWeek.Friday,
        System.DayOfWeek.Saturday => DayOfWeek.Saturday,
        System.DayOfWeek.Sunday => DayOfWeek.Sunday,
        _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
    };
}