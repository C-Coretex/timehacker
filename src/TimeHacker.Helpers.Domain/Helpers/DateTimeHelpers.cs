namespace TimeHacker.Helpers.Domain.Helpers;

public static class DateTimeHelpers
{
    public static DateOnly Min(DateOnly a, DateOnly b) => a < b ? a : b;
    public static DateOnly Max(DateOnly a, DateOnly b) => a > b ? a : b;
}
