using System.Globalization;

using DayOfWeekEnum = TimeHacker.Domain.Models.EntityModels.Enums.DayOfWeek;

namespace TimeHacker.Application.Api.Tests.RichModelTests;

/// <summary>
/// Direct unit tests for <see cref="IRepeatingEntityType.GetNextTaskDate"/> on each of the four
/// repeating-entity types, isolating the next-date logic (week-wrap, short-month skip, leap-year day-366 skip)
/// that is otherwise only exercised transitively through <c>GetNextEntityDatesIn</c>.
/// </summary>
public class RepeatingEntityTypeTests
{
    #region DayRepeatingEntity

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(35)]
    [Trait("DayRepeatingEntity", "GetNextTaskDate adds the configured day count")]
    public void DayRepeatingEntity_GetNextTaskDate_AddsDayCount(int days)
    {
        var entity = new DayRepeatingEntity(days);
        var start = new DateOnly(2024, 01, 01);

        entity.GetNextTaskDate(start).Should().Be(start.AddDays(days));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("DayRepeatingEntity", "Throws on day count below 1")]
    public void DayRepeatingEntity_ShouldThrowOnInvalidDayCount(int days)
    {
        Assert.Throws<ArgumentException>(() => new DayRepeatingEntity(days));
    }

    #endregion

    #region WeekRepeatingEntity

    // 2024-09-16 is a Monday; the days below let the assertions read by weekday name.
    private static readonly DateOnly Monday = new(2024, 09, 16);
    private static readonly DateOnly Wednesday = new(2024, 09, 18);
    private static readonly DateOnly Friday = new(2024, 09, 20);
    private static readonly DateOnly Sunday = new(2024, 09, 22);

    [Fact]
    [Trait("WeekRepeatingEntity", "Single day - earlier weekday returns same week")]
    public void WeekRepeatingEntity_SingleDay_EarlierWeekday_ReturnsSameWeek()
    {
        var entity = new WeekRepeatingEntity([DayOfWeekEnum.Wednesday]);

        entity.GetNextTaskDate(Monday).Should().Be(Wednesday);
    }

    [Fact]
    [Trait("WeekRepeatingEntity", "Single day - same weekday wraps a full week")]
    public void WeekRepeatingEntity_SingleDay_SameWeekday_WrapsAWeek()
    {
        var entity = new WeekRepeatingEntity([DayOfWeekEnum.Wednesday]);

        entity.GetNextTaskDate(Wednesday).Should().Be(Wednesday.AddDays(7));
    }

    [Fact]
    [Trait("WeekRepeatingEntity", "Single day - later weekday wraps to next week")]
    public void WeekRepeatingEntity_SingleDay_LaterWeekday_WrapsToNextWeek()
    {
        var entity = new WeekRepeatingEntity([DayOfWeekEnum.Wednesday]);

        entity.GetNextTaskDate(Friday).Should().Be(Wednesday.AddDays(7));
    }

    [Fact]
    [Trait("WeekRepeatingEntity", "Multiple days - steps to the next selected day")]
    public void WeekRepeatingEntity_MultipleDays_StepsToNextSelectedDay()
    {
        var entity = new WeekRepeatingEntity([DayOfWeekEnum.Monday, DayOfWeekEnum.Wednesday, DayOfWeekEnum.Friday]);

        entity.GetNextTaskDate(Monday).Should().Be(Wednesday);
        entity.GetNextTaskDate(Wednesday).Should().Be(Friday);
    }

    [Fact]
    [Trait("WeekRepeatingEntity", "Multiple days - last selected day wraps to first of next week")]
    public void WeekRepeatingEntity_MultipleDays_LastDay_WrapsToFirstOfNextWeek()
    {
        var entity = new WeekRepeatingEntity([DayOfWeekEnum.Monday, DayOfWeekEnum.Wednesday, DayOfWeekEnum.Friday]);

        // Friday -> next Monday (wrap), and Sunday (.NET DayOfWeek == 0) normalizes to 7 -> next Monday too.
        entity.GetNextTaskDate(Friday).Should().Be(Monday.AddDays(7));
        entity.GetNextTaskDate(Sunday).Should().Be(Monday.AddDays(7));
    }

    [Fact]
    [Trait("WeekRepeatingEntity", "Sunday normalization (DayOfWeek 0 -> 7)")]
    public void WeekRepeatingEntity_SundayNormalization()
    {
        var entity = new WeekRepeatingEntity([DayOfWeekEnum.Sunday]);

        // Monday -> this week's Sunday; Sunday -> next Sunday (+7).
        entity.GetNextTaskDate(Monday).Should().Be(Sunday);
        entity.GetNextTaskDate(Sunday).Should().Be(Sunday.AddDays(7));
    }

    [Fact]
    [Trait("WeekRepeatingEntity", "Constructor sorts unordered input")]
    public void WeekRepeatingEntity_UnorderedInput_YieldsSameResultAsOrdered()
    {
        var unordered = new WeekRepeatingEntity([DayOfWeekEnum.Friday, DayOfWeekEnum.Monday, DayOfWeekEnum.Wednesday]);
        var ordered = new WeekRepeatingEntity([DayOfWeekEnum.Monday, DayOfWeekEnum.Wednesday, DayOfWeekEnum.Friday]);

        unordered.GetNextTaskDate(Monday).Should().Be(ordered.GetNextTaskDate(Monday));
        unordered.RepeatsOn.Should().Equal(ordered.RepeatsOn);
    }

    #endregion

    #region MonthRepeatingEntity

    [Theory]
    [InlineData("2024-01-01", "2024-01-15")] // before target day -> same month
    [InlineData("2024-01-15", "2024-02-15")] // on target day -> next month
    [InlineData("2024-01-20", "2024-02-15")] // after target day -> next month
    [Trait("MonthRepeatingEntity", "Day 15 picks correct month")]
    public void MonthRepeatingEntity_Day15(string from, string expected)
    {
        var entity = new MonthRepeatingEntity(15);

        entity.GetNextTaskDate(DateOnly.Parse(from, CultureInfo.InvariantCulture))
            .Should().Be(DateOnly.Parse(expected, CultureInfo.InvariantCulture));
    }

    [Fact]
    [Trait("MonthRepeatingEntity", "Day 31 skips short months")]
    public void MonthRepeatingEntity_Day31_SkipsShortMonths()
    {
        var entity = new MonthRepeatingEntity(31);

        // Jan 31 -> Mar 31 (Feb skipped) -> May 31 (Apr skipped).
        var first = entity.GetNextTaskDate(new DateOnly(2023, 01, 31));
        first.Should().Be(new DateOnly(2023, 03, 31));
        entity.GetNextTaskDate(first!.Value).Should().Be(new DateOnly(2023, 05, 31));
    }

    [Fact]
    [Trait("MonthRepeatingEntity", "Day 30 skips February")]
    public void MonthRepeatingEntity_Day30_SkipsFebruary()
    {
        var entity = new MonthRepeatingEntity(30);

        entity.GetNextTaskDate(new DateOnly(2023, 01, 31)).Should().Be(new DateOnly(2023, 03, 30));
    }

    [Fact]
    [Trait("MonthRepeatingEntity", "Day 29 skips non-leap February")]
    public void MonthRepeatingEntity_Day29_NonLeapYear_SkipsFebruary()
    {
        var entity = new MonthRepeatingEntity(29);

        entity.GetNextTaskDate(new DateOnly(2023, 01, 29)).Should().Be(new DateOnly(2023, 03, 29));
    }

    [Fact]
    [Trait("MonthRepeatingEntity", "Day 29 produces Feb 29 in a leap year")]
    public void MonthRepeatingEntity_Day29_LeapYear_ProducesFeb29()
    {
        var entity = new MonthRepeatingEntity(29);

        entity.GetNextTaskDate(new DateOnly(2024, 01, 29)).Should().Be(new DateOnly(2024, 02, 29));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    [Trait("MonthRepeatingEntity", "Throws on out-of-range day")]
    public void MonthRepeatingEntity_ShouldThrowOnInvalidDay(byte day)
    {
        Assert.Throws<ArgumentException>(() => new MonthRepeatingEntity(day));
    }

    #endregion

    #region YearRepeatingEntity

    [Fact]
    [Trait("YearRepeatingEntity", "Day 200 picks correct year")]
    public void YearRepeatingEntity_Day200()
    {
        var entity = new YearRepeatingEntity(200);

        // Day 200 of 2024 is before today-or-past from Jan 1 -> stays in 2024.
        var first = entity.GetNextTaskDate(new DateOnly(2024, 01, 01));
        first.Should().Be(new DateOnly(2024, 01, 01).AddDays(199));

        // From that occurrence -> day 200 of the next year.
        entity.GetNextTaskDate(first!.Value).Should().Be(new DateOnly(2025, 01, 01).AddDays(199));
    }

    [Fact]
    [Trait("YearRepeatingEntity", "Day 366 skips non-leap years")]
    public void YearRepeatingEntity_Day366_SkipsNonLeapYears()
    {
        var entity = new YearRepeatingEntity(366);

        // Day 366 of leap 2024 is Dec 31; the next occurrence skips 2025/26/27 to leap 2028.
        var first = entity.GetNextTaskDate(new DateOnly(2024, 01, 01));
        first.Should().Be(new DateOnly(2024, 12, 31));
        entity.GetNextTaskDate(first!.Value).Should().Be(new DateOnly(2028, 12, 31));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(367)]
    [Trait("YearRepeatingEntity", "Throws on out-of-range day")]
    public void YearRepeatingEntity_ShouldThrowOnInvalidDay(int day)
    {
        Assert.Throws<ArgumentException>(() => new YearRepeatingEntity(day));
    }

    #endregion

    #region OnceRepeatingEntity

    [Fact]
    [Trait("OnceRepeatingEntity", "Walks the chosen dates in order")]
    public void OnceRepeatingEntity_GetNextTaskDate_WalksDatesInOrder()
    {
        var first = new DateOnly(2026, 08, 15);
        var second = new DateOnly(2026, 08, 20);
        var third = new DateOnly(2026, 09, 01);

        // Deliberately unsorted: the constructor is responsible for ordering.
        var entity = new OnceRepeatingEntity([third, first, second]);

        entity.GetNextTaskDate(first.AddDays(-1)).Should().Be(first);
        entity.GetNextTaskDate(first).Should().Be(second);
        entity.GetNextTaskDate(second).Should().Be(third);
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "Sorts and de-duplicates the chosen dates")]
    public void OnceRepeatingEntity_ShouldSortAndDeduplicateDates()
    {
        var duplicate = new DateOnly(2026, 08, 20);
        var earlier = new DateOnly(2026, 08, 15);

        var entity = new OnceRepeatingEntity([duplicate, earlier, duplicate]);

        entity.Dates.Should().Equal(earlier, duplicate);
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "Returns null once the dates are exhausted")]
    public void OnceRepeatingEntity_GetNextTaskDate_ReturnsNullValueWhenExhausted()
    {
        var only = new DateOnly(2026, 08, 15);
        var entity = new OnceRepeatingEntity([only]);

        entity.GetNextTaskDate(only).Should().BeNull();
        entity.GetNextTaskDate(only.AddYears(10)).Should().BeNull();
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "Throws when no date is chosen")]
    public void OnceRepeatingEntity_ShouldThrowOnEmptyDates()
    {
        Assert.Throws<ArgumentException>(() => new OnceRepeatingEntity([]));
    }

    #endregion
}
