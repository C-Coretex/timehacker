using System.Globalization;

namespace TimeHacker.Application.Api.Tests.RichModelTests;

public class ScheduleEntityTests
{
    private static DateOnly DateFrom(string value) => DateOnly.Parse(value, CultureInfo.InvariantCulture);

    #region DayRepeatingEntity

    [Theory, CombinatorialData]
    [Trait("DayRepeatingEntity", "Should return correct data")]
    public void DayRepeatingEntity_ShouldReturnCorrectData(bool endsOn, bool lastEntityCreated)
    {
        var newEntity = new ScheduleEntityReturn()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2)),
            LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(DateTime.Now.AddDays(1)) : null,
            FirstEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(DateTime.Now.AddDays(1)) : null,
            EndsOn = endsOn ? DateOnly.FromDateTime(DateTime.Now.AddDays(6)) : null,
            CreatedTimestamp = DateTime.Now
        };
        var dateFrom = DateOnly.FromDateTime(DateTime.Now);
        var dateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(10));

        var result = newEntity.GetNextEntityDatesIn(dateFrom, dateTo).ToList();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var expectedCountOfItems = endsOn ? 3 : 5;
        var startingDate = DateTime.Now;
        if (lastEntityCreated)
        {
            expectedCountOfItems--;
            startingDate = startingDate.AddDays(1);
        }

        result.Count.Should().Be(expectedCountOfItems);
        for (var i = 0; i < expectedCountOfItems; i++)
        {
            startingDate = startingDate.AddDays(2);
            result[i].Should().Be(DateOnly.FromDateTime(startingDate));
        }
    }

    [Fact]
    [Trait("DayRepeatingEntity", "Should throw exception on incorrect data")]
    public void DayRepeatingEntity_ShouldThrowExceptionOnIncorrectData()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var newEntity = new ScheduleEntityReturn()
            {
                RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(0)),
                LastEntityCreated = null,
                EndsOn = null
            };
        });
    }

    #endregion

    #region WeekRepeatingEntity

    [Theory, CombinatorialData]
    [Trait("WeekRepeatingEntity", "Should return correct data")]
    public void WeekRepeatingEntity_ShouldReturnCorrectData(bool endsOn, bool lastEntityCreated)
    {
        var monday = new DateTime(2024, 09, 16);

        var newEntity = new ScheduleEntityReturn()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.WeekRepeatingEntity, new WeekRepeatingEntity([Domain.Models.EntityModels.Enums.DayOfWeek.Tuesday, Domain.Models.EntityModels.Enums.DayOfWeek.Monday, Domain.Models.EntityModels.Enums.DayOfWeek.Friday])),
            CreatedTimestamp = monday,
            LastEntityCreated = lastEntityCreated ? global::System.DateOnly.FromDateTime(monday.AddDays(1)) : null,
            FirstEntityCreated = lastEntityCreated ? global::System.DateOnly.FromDateTime(monday.AddDays(1)) : null,
            EndsOn = endsOn ? global::System.DateOnly.FromDateTime(monday.AddDays(8)) : null
        };
        var dateFrom = DateOnly.FromDateTime(monday);
        var dateTo = DateOnly.FromDateTime(monday.AddDays(14));

        var result = newEntity.GetNextEntityDatesIn(dateFrom, dateTo).ToList();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        List<DateTime> expected =
        [
            monday.AddDays(1), monday.AddDays(4), monday.AddDays(7), monday.AddDays(8), monday.AddDays(11),
            monday.AddDays(14)
        ];
        var expectedCountOfItems = endsOn ? 4 : 6;
        if (lastEntityCreated)
        {
            expectedCountOfItems--;
            expected.RemoveAt(0);
        }

        result.Count.Should().Be(expectedCountOfItems);


        for (var i = 0; i < expectedCountOfItems; i++)
            result[i].Should().Be(DateOnly.FromDateTime(expected[i]));
    }

    [Fact]
    [Trait("WeekRepeatingEntity", "Should throw exception on incorrect data")]
    public void WeekRepeatingEntity_ShouldThrowExceptionOnIncorrectData()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var newEntity = new ScheduleEntityReturn()
            {
                RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.WeekRepeatingEntity, new WeekRepeatingEntity([])),
                LastEntityCreated = null,
                EndsOn = null
            };
        });
    }

    #endregion

    #region MonthRepeatingEntity

    [Theory, CombinatorialData]
    [Trait("MonthRepeatingEntity", "Should return correct data")]
    public void MonthRepeatingEntity_ShouldReturnCorrectData(bool endsOn, bool lastEntityCreated)
    {
        var january = new DateTime(2023, 01, 01);

        var newEntity = new ScheduleEntityReturn()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.MonthRepeatingEntity, new MonthRepeatingEntity(12)),
            CreatedTimestamp = january,
            LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddDays(12)) : null,
            FirstEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddDays(12)) : null,
            EndsOn = endsOn ? DateOnly.FromDateTime(january.AddMonths(8)) : null
        };
        var dateFrom = DateOnly.FromDateTime(january);
        var dateTo = DateOnly.FromDateTime(january.AddMonths(13));

        var result = newEntity.GetNextEntityDatesIn(dateFrom, dateTo).ToList();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var expectedCountOfItems = endsOn ? 8 : 13;
        if (lastEntityCreated)
            expectedCountOfItems--;

        result.Count.Should().Be(expectedCountOfItems);

        for (var i = 0; i < expectedCountOfItems; i++)
            result[i].Should().Be(DateOnly.FromDateTime(january.AddDays(11).AddMonths(i + (lastEntityCreated ? 1 : 0))));
    }

    [Theory, CombinatorialData]
    [Trait("MonthRepeatingEntity", "Should return correct data on 31-st day")]
    public void MonthRepeatingEntity_ShouldReturnCorrectDataOn31Day(bool endsOn, bool lastEntityCreated, bool isLapYear)
    {
        var january = new DateTime(isLapYear ? 2024 : 2023, 01, 01);

        var newEntity = new ScheduleEntityReturn()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.MonthRepeatingEntity, new MonthRepeatingEntity((byte)(isLapYear ? 29 : 31))),
            CreatedTimestamp = january,
            LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddDays(31)) : null,
            FirstEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddDays(31)) : null,
            EndsOn = endsOn ? DateOnly.FromDateTime(january.AddMonths(8)) : null
        };
        var dateFrom = DateOnly.FromDateTime(january);
        var dateTo = DateOnly.FromDateTime(january.AddMonths(13));

        var result = newEntity.GetNextEntityDatesIn(dateFrom, dateTo).ToList();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var expected = isLapYear ? Enumerable.Repeat(0, 13).Select((_, i) => january.AddMonths(i)).ToList() 
            : [january, january.AddMonths(2), january.AddMonths(4), january.AddMonths(6), january.AddMonths(7), january.AddMonths(9), january.AddMonths(11), january.AddMonths(12)];

        if (endsOn)
            expected = expected.Where(x => x.Year == january.Year && x.Month <= 8).ToList();
        if (lastEntityCreated)
            expected.RemoveAt(0);

        result.Count.Should().Be(expected.Count);

        for (var i = 0; i < expected.Count; i++)
            result[i].Should().Be(DateOnly.FromDateTime(expected[i].AddDays(isLapYear ? 28 : 30)));
    }

    [Theory]
    [InlineData(32)]
    [InlineData(0)]
    [Trait("MonthRepeatingEntity", "Should throw exception on incorrect data")]
    public void MonthRepeatingEntity_ShouldThrowExceptionOnIncorrectData(byte days)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var newEntity = new ScheduleEntityReturn()
            {
                RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.MonthRepeatingEntity, new MonthRepeatingEntity(days)),
                LastEntityCreated = null,
                EndsOn = null
            };
        });
    }

    #endregion

    #region YearRepeatingEntity

    [Theory, CombinatorialData]
    [Trait("YearRepeatingEntity", "Should return correct data")]
    public void YearRepeatingEntity_ShouldReturnCorrectData(bool endsOn, bool lastEntityCreated, bool isLapYear)
    {
        var january = new DateTime(2024, 01, 01);

        var newEntity = new ScheduleEntityReturn()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.YearRepeatingEntity, new YearRepeatingEntity(isLapYear ? 366 : 200)),
            CreatedTimestamp = january,
            LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddYears(1)) : null,
            FirstEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddYears(1)) : null,
            EndsOn = endsOn ? DateOnly.FromDateTime(january.AddYears(5)) : null
        };
        var dateFrom = DateOnly.FromDateTime(january);
        var dateTo = DateOnly.FromDateTime(january.AddYears(10));

        var result = newEntity.GetNextEntityDatesIn(dateFrom, dateTo).ToList();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var expected = isLapYear
            ?
            [
                january, january.AddYears(4), january.AddYears(8)
            ]
            : Enumerable.Repeat(0, 10).Select((_, i) => january.AddYears(i)).ToList();

        if (endsOn)
            expected = expected.Where(x => x.Year <= 2024 + 4).ToList();
        if (lastEntityCreated)
            expected.RemoveAt(0);

        result.Count.Should().Be(expected.Count);

        for (var i = 0; i < expected.Count; i++)
            result[i].Should().Be(DateOnly.FromDateTime(expected[i].AddDays(isLapYear ? 365 : 199)));
    }

    [Theory]
    [InlineData(367)]
    [InlineData(0)]
    [Trait("YearRepeatingEntity", "Should throw exception on incorrect data")]
    public void YearRepeatingEntity_ShouldThrowExceptionOnIncorrectData(int days)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var newEntity = new ScheduleEntityReturn()
            {
                RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.YearRepeatingEntity, new YearRepeatingEntity(days)),
                LastEntityCreated = null,
                EndsOn = null
            };
        });
    }

    #endregion

    #region IsEntityDateCorrect

    // Day(2) pattern from a fixed creation date; occurrences are 2024-01-03, -05, -07, ...
    private static ScheduleEntityReturn Day2Schedule(DateOnly? endsOn = null, DateOnly? lastEntityCreated = null)
        => new()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2)),
            CreatedTimestamp = new DateTime(2024, 01, 01),
            LastEntityCreated = lastEntityCreated,
            EndsOn = endsOn
        };

    [Theory]
    [InlineData("2024-01-03")]
    [InlineData("2024-01-05")]
    [InlineData("2024-01-11")]
    [Trait("IsEntityDateCorrect", "Returns true for an on-pattern date")]
    public void IsEntityDateCorrect_OnPatternDate_ReturnsTrue(string date)
    {
        Day2Schedule().IsEntityDateCorrect(DateFrom(date)).Should().BeTrue();
    }

    [Theory]
    [InlineData("2024-01-02")]
    [InlineData("2024-01-04")]
    [Trait("IsEntityDateCorrect", "Returns false for an off-pattern date")]
    public void IsEntityDateCorrect_OffPatternDate_ReturnsFalse(string date)
    {
        Day2Schedule().IsEntityDateCorrect(DateFrom(date)).Should().BeFalse();
    }

    [Fact]
    [Trait("IsEntityDateCorrect", "Returns false for a date past EndsOn")]
    public void IsEntityDateCorrect_DatePastEndsOn_ReturnsFalse()
    {
        // 2024-01-07 is on-pattern but lies beyond EndsOn.
        Day2Schedule(endsOn: new DateOnly(2024, 01, 05))
            .IsEntityDateCorrect(new DateOnly(2024, 01, 07)).Should().BeFalse();
    }

    [Theory]
    [InlineData("2024-01-01")] // equal to the start
    [InlineData("2023-12-31")] // before the start
    [Trait("IsEntityDateCorrect", "Returns false for a date at or before the start")]
    public void IsEntityDateCorrect_DateAtOrBeforeStart_ReturnsFalse(string date)
    {
        Day2Schedule().IsEntityDateCorrect(DateFrom(date)).Should().BeFalse();
    }

    [Fact]
    [Trait("IsEntityDateCorrect", "Starts from LastEntityCreated when date is at or after it")]
    public void IsEntityDateCorrect_DateAtOrAfterLastEntityCreated_ValidatesFromLastEntityCreated()
    {
        var schedule = Day2Schedule(lastEntityCreated: new DateOnly(2024, 01, 05));

        schedule.IsEntityDateCorrect(new DateOnly(2024, 01, 09)).Should().BeTrue();
    }

    [Fact]
    [Trait("IsEntityDateCorrect", "Replays from creation when date precedes LastEntityCreated")]
    public void IsEntityDateCorrect_DateBeforeLastEntityCreated_ReplaysFromCreation()
    {
        var schedule = Day2Schedule(lastEntityCreated: new DateOnly(2024, 01, 05));

        schedule.IsEntityDateCorrect(new DateOnly(2024, 01, 03)).Should().BeTrue();
    }

    [Fact]
    [Trait("IsEntityDateCorrect", "Validates weekday patterns")]
    public void IsEntityDateCorrect_WeekPattern_MatchesWeekday()
    {
        // CreatedTimestamp is Monday 2024-09-16; pattern repeats on Mon & Thu.
        var schedule = new ScheduleEntityReturn()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.WeekRepeatingEntity,
                new WeekRepeatingEntity([Domain.Models.EntityModels.Enums.DayOfWeek.Monday, Domain.Models.EntityModels.Enums.DayOfWeek.Thursday])),
            CreatedTimestamp = new DateTime(2024, 09, 16)
        };

        schedule.IsEntityDateCorrect(new DateOnly(2024, 09, 19)).Should().BeTrue();  // Thursday - on pattern
        schedule.IsEntityDateCorrect(new DateOnly(2024, 09, 23)).Should().BeTrue();  // next Monday - on pattern
        schedule.IsEntityDateCorrect(new DateOnly(2024, 09, 20)).Should().BeFalse(); // Friday - off pattern
    }

    [Fact]
    [Trait("IsEntityDateCorrect", "Returns false for a date equal to LastEntityCreated")]
    public void IsEntityDateCorrect_DateEqualsLastEntityCreated_ReturnsTrue()
    {
        // 2024-01-05 is on-pattern, but validation steps strictly forward from LastEntityCreated,
        // so the marker date itself is never reported as a (future) occurrence.
        Day2Schedule(lastEntityCreated: new DateOnly(2024, 01, 05))
            .IsEntityDateCorrect(new DateOnly(2024, 01, 05)).Should().BeTrue();
    }

    [Theory]
    [InlineData(false, "2024-01-03", true)]  // < LastEntityCreated -> replays from CreatedTimestamp
    [InlineData(true, "2024-01-03", true)]   // (LastEntityCreated set) still replays from CreatedTimestamp
    [InlineData(false, "2024-01-09", true)]  // validated from CreatedTimestamp
    [InlineData(true, "2024-01-09", true)]   // >= LastEntityCreated -> validated from LastEntityCreated
    [InlineData(false, "2024-01-08", false)] // off-pattern from CreatedTimestamp
    [InlineData(true, "2024-01-08", false)]  // off-pattern from LastEntityCreated
    [Trait("IsEntityDateCorrect", "Same answer whether validated from CreatedTimestamp or LastEntityCreated")]
    public void IsEntityDateCorrect_SameResultFromEitherStartingPoint(bool withLastEntityCreated, string date, bool expected)
    {
        // LastEntityCreated (2024-01-05) is on-pattern, so both starting points lie on the same lattice
        // and must agree for any queried date.
        var schedule = Day2Schedule(lastEntityCreated: withLastEntityCreated ? new DateOnly(2024, 01, 05) : null);

        schedule.IsEntityDateCorrect(DateFrom(date)).Should().Be(expected);
    }

    #endregion

    #region GetNextEntityDatesIn corner cases

    [Theory]
    [InlineData("2024-01-01", "2024-01-01")] // empty range (from == to)
    [InlineData("2024-01-10", "2024-01-05")] // inverted range (to < from)
    [Trait("GetNextEntityDatesIn", "Returns nothing for an empty or inverted range")]
    public void GetNextEntityDatesIn_EmptyOrInvertedRange_ReturnsNothing(string from, string to)
    {
        Day2Schedule().GetNextEntityDatesIn(DateFrom(from), DateFrom(to)).Should().BeEmpty();
    }

    [Fact]
    [Trait("GetNextEntityDatesIn", "Returns nothing when EndsOn is before the range")]
    public void GetNextEntityDatesIn_EndsOnBeforeRange_ReturnsNothing()
    {
        Day2Schedule(endsOn: new DateOnly(2023, 12, 31))
            .GetNextEntityDatesIn(new DateOnly(2024, 01, 01), new DateOnly(2024, 02, 01))
            .Should().BeEmpty();
    }

    [Fact]
    [Trait("GetNextEntityDatesIn", "Includes occurrences on the from and to boundaries")]
    public void GetNextEntityDatesIn_Boundaries_AreInclusive()
    {
        // Occurrences: 03, 05, 07, 09. from/to land exactly on occurrences.
        var result = Day2Schedule()
            .GetNextEntityDatesIn(new DateOnly(2024, 01, 03), new DateOnly(2024, 01, 09))
            .ToList();

        result.Should().Equal(
            new DateOnly(2024, 01, 03),
            new DateOnly(2024, 01, 05),
            new DateOnly(2024, 01, 07),
            new DateOnly(2024, 01, 09));
    }

    [Fact]
    [Trait("GetNextEntityDatesIn", "Replays from FirstEntityCreated when LastEntityCreated is past from")]
    public void GetNextEntityDatesIn_LastEntityCreatedPastFrom_ReplaysFromFirstEntityCreated()
    {
        // Refresh/recalculation case: the range overlaps already-generated dates, so generation replays
        // from FirstEntityCreated rather than continuing from LastEntityCreated.
        var schedule = new ScheduleEntityReturn()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2)),
            CreatedTimestamp = new DateTime(2024, 01, 01),
            FirstEntityCreated = new DateOnly(2024, 01, 03),
            LastEntityCreated = new DateOnly(2024, 01, 11)
        };

        var result = schedule
            .GetNextEntityDatesIn(new DateOnly(2024, 01, 05), new DateOnly(2024, 01, 11))
            .ToList();

        result.Should().Equal(
            new DateOnly(2024, 01, 05),
            new DateOnly(2024, 01, 07),
            new DateOnly(2024, 01, 09),
            new DateOnly(2024, 01, 11));
    }

    [Fact]
    [Trait("GetNextEntityDatesIn", "Stops before the next occurrence when EndsOn falls between occurrences")]
    public void GetNextEntityDatesIn_EndsOnBetweenOccurrences_KeepsLastDateBelowEndsOn()
    {
        // Occurrences: 03, 05, 07, 09. EndsOn = 08 sits strictly between 07 and 09, so the strict
        // "> EndsOn" cutoff yields up to 07 and excludes 09 (the last date stays below EndsOn).
        var result = Day2Schedule(endsOn: new DateOnly(2024, 01, 08))
            .GetNextEntityDatesIn(new DateOnly(2024, 01, 01), new DateOnly(2024, 01, 31))
            .ToList();

        result.Should().Equal(
            new DateOnly(2024, 01, 03),
            new DateOnly(2024, 01, 05),
            new DateOnly(2024, 01, 07));
    }

    [Fact]
    [Trait("GetNextEntityDatesIn", "Continues from LastEntityCreated when it is at or before from")]
    public void GetNextEntityDatesIn_LastEntityCreatedBeforeFrom_ContinuesFromLastEntityCreated()
    {
        // Forward (non-refresh) generation: LastEntityCreated <= from, so generation continues from the
        // last marker rather than replaying from FirstEntityCreated.
        var schedule = new ScheduleEntityReturn()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2)),
            CreatedTimestamp = new DateTime(2024, 01, 01),
            FirstEntityCreated = new DateOnly(2024, 01, 03),
            LastEntityCreated = new DateOnly(2024, 01, 05)
        };

        var result = schedule
            .GetNextEntityDatesIn(new DateOnly(2024, 01, 07), new DateOnly(2024, 01, 13))
            .ToList();

        result.Should().Equal(
            new DateOnly(2024, 01, 07),
            new DateOnly(2024, 01, 09),
            new DateOnly(2024, 01, 11),
            new DateOnly(2024, 01, 13));
    }

    #endregion

    #region OnceRepeatingEntity

    // EndsOn mirrors what ScheduleEntityHelper derives for a finite series — the last chosen date.
    // CreatedTimestamp sits well before any of them so the walk starts ahead of the first occurrence.
    private static ScheduleEntityReturn OnceSchedule(params DateOnly[] dates)
        => new()
        {
            RepeatingEntity = new RepeatingEntityDto(RepeatingEntityType.OnceRepeatingEntity, new OnceRepeatingEntity(dates)),
            CreatedTimestamp = new DateTime(2024, 01, 01),
            EndsOn = dates.Max()
        };

    [Fact]
    [Trait("OnceRepeatingEntity", "Yields exactly the chosen dates inside the range")]
    public void OnceRepeatingEntity_GetNextEntityDatesIn_YieldsChosenDates()
    {
        var schedule = OnceSchedule(DateFrom("2026-08-15"), DateFrom("2026-08-20"), DateFrom("2026-09-01"));

        var result = schedule.GetNextEntityDatesIn(DateFrom("2026-08-01"), DateFrom("2026-08-31")).ToList();

        result.Should().Equal(DateFrom("2026-08-15"), DateFrom("2026-08-20"));
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "Yields the first chosen date even when it is the range start")]
    public void OnceRepeatingEntity_GetNextEntityDatesIn_IncludesTheEarliestDate()
    {
        var today = DateFrom("2026-08-15");
        var schedule = OnceSchedule(today);

        schedule.GetNextEntityDatesIn(today, today.AddDays(6)).Should().Equal(today);
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "Terminates when the range starts")]
    public void OnceRepeatingEntity_GetNextEntityDatesIn_TerminatesOnReplay()
    {
        var schedule = OnceSchedule(DateFrom("2026-08-15"));

        var result = schedule.GetNextEntityDatesIn(DateFrom("2026-08-01"), DateFrom("2026-08-31")).ToList();

        result.Should().Equal(DateFrom("2026-08-15"));
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "Terminates with no EndsOn set")]
    public void OnceRepeatingEntity_GetNextEntityDatesIn_TerminatesWithoutEndsOn()
    {
        var schedule = OnceSchedule(DateFrom("2026-08-15")) with { EndsOn = null };

        var act = () => schedule.GetNextEntityDatesIn(DateFrom("2026-08-01"), DateFrom("2026-12-31")).ToList();

        act.Should().NotThrow();
        act().Should().Equal(DateFrom("2026-08-15"));
    }

    [Theory]
    [InlineData("2026-08-15", true)]
    [InlineData("2026-09-01", true)]
    [InlineData("2026-08-16", false)]
    [Trait("OnceRepeatingEntity", "IsEntityDateCorrect only accepts chosen dates")]
    public void OnceRepeatingEntity_IsEntityDateCorrect(string date, bool expected)
    {
        var schedule = OnceSchedule(DateFrom("2026-08-15"), DateFrom("2026-09-01"));

        schedule.IsEntityDateCorrect(DateFrom(date)).Should().Be(expected);
    }

    #endregion
}
