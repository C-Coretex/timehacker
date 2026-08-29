using Microsoft.Extensions.Time.Testing;

using DayOfWeekEnum = TimeHacker.Domain.Models.EntityModels.Enums.DayOfWeek;

namespace TimeHacker.Application.Api.Tests.RichModelTests;

public class InputScheduleEntityModelTests
{
    // 2024-01-01 (a Monday) as a fixed clock so EndsOn math is deterministic.
    private static readonly DateOnly FixedToday = new(2024, 01, 01);
    private static readonly FakeTimeProvider FixedClock = new(new DateTimeOffset(FixedToday.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));

    [Fact]
    [Trait("DayRepeatingEntity", "Should return correct data without EndsOnModel")]
    public void GetScheduleEntity_ShouldReturnCorrectDataWithoutEndsOnModel()
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity());

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, null, DateOnly.FromDateTime(DateTime.UtcNow), TimeProvider.System);
        scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);
        scheduledEntity.EndsOn.Should().BeNull();
    }

    [Fact]
    [Trait("DayRepeatingEntity", "Should return correct data with EndsOnModel without MaxOccurrences")]
    public void GetScheduleEntity_ShouldReturnCorrectDataWithEndsOnModelWithoutMaxOccurrences()
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity());

        var maxDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
        var endsOnModel = new EndsOnModel()
        {
            MaxDate = maxDate,
            MaxOccurrences = null
        };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, DateOnly.FromDateTime(DateTime.UtcNow), TimeProvider.System);
        scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);
        scheduledEntity.EndsOn.Should().Be(maxDate);
    }

    [Theory, CombinatorialData]
    [Trait("DayRepeatingEntity", "Should return correct data with EndsOnModel with MaxOccurrences")]
    public void GetScheduleEntity_ShouldReturnCorrectDataWithEndsOnModelWithMaxOccurrences([CombinatorialValues(0, 1, 5, 10)] uint maxOccurrences, bool isMaxDate)
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));

        var maxDate = isMaxDate ? (DateOnly?)DateOnly.FromDateTime(DateTime.Now.AddDays(8)) : null;
        var endsOnModel = new EndsOnModel()
        {
            MaxDate = maxDate,
            MaxOccurrences = maxOccurrences
        };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, DateOnly.FromDateTime(DateTime.UtcNow), TimeProvider.System);
        scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);

        var endsOn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(maxOccurrences * 2));

        if (isMaxDate && endsOn > maxDate)
            endsOn = maxDate.Value;
        scheduledEntity.EndsOn.Should().Be(endsOn);
    }

    // Expected EndsOn after stepping each pattern 3 times from FixedToday (2024-01-01, a Monday).
    public static IEnumerable<object[]> MaxOccurrencesPatterns =>
    [
        // Day(2): 01-01 +2 +2 +2.
        [new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2)), new DateOnly(2024, 01, 07)],
        // Week[Mon,Thu]: Mon -> Thu 01-04 -> Mon 01-08 -> Thu 01-11.
        [new RepeatingEntityDto(RepeatingEntityType.WeekRepeatingEntity, new WeekRepeatingEntity([DayOfWeekEnum.Monday, DayOfWeekEnum.Thursday])), new DateOnly(2024, 01, 11)],
        // Month(15): Jan 15 -> Feb 15 -> Mar 15.
        [new RepeatingEntityDto(RepeatingEntityType.MonthRepeatingEntity, new MonthRepeatingEntity(15)), new DateOnly(2024, 03, 15)],
        // Year(200): day 200 of leap 2024 (07-18) -> 2025 (07-19) -> 2026 (07-19).
        [new RepeatingEntityDto(RepeatingEntityType.YearRepeatingEntity, new YearRepeatingEntity(200)), new DateOnly(2026, 07, 19)]
    ];

    [Theory]
    [MemberData(nameof(MaxOccurrencesPatterns))]
    [Trait("GetScheduleEntity", "MaxOccurrences resolves EndsOn for each pattern")]
    public void GetScheduleEntity_MaxOccurrences_ResolvesEndsOn(RepeatingEntityDto repeatingEntityModel, DateOnly expectedEndsOn)
    {
        ArgumentNullException.ThrowIfNull(repeatingEntityModel);

        var endsOnModel = new EndsOnModel { MaxOccurrences = 3 };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(expectedEndsOn);
    }

    [Fact]
    [Trait("GetScheduleEntity", "MaxDate earlier than the occurrence date clamps EndsOn")]
    public void GetScheduleEntity_MaxDateEarlierThanOccurrence_ClampsToMaxDate()
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));
        var maxDate = FixedToday.AddDays(5);
        var endsOnModel = new EndsOnModel { MaxOccurrences = 10, MaxDate = maxDate };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(maxDate);
    }

    [Theory]
    [InlineData(true), InlineData(false)]
    [Trait("GetScheduleEntity", "Empty EndsOnModel (no MaxOccurrences, no MaxDate) yields null EndsOn")]
    public void GetScheduleEntity_EmptyEndsOnModel_YieldsNullEndsOn(bool includeEndsOnModel)
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));
        var endsOnModel = includeEndsOnModel ? new EndsOnModel { MaxOccurrences = null, MaxDate = null } : null;

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().BeNull();
    }

    [Fact]
    [Trait("GetScheduleEntity", "Zero MaxOccurrences yields today")]
    public void GetScheduleEntity_ZeroMaxOccurrences_YieldsToday()
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));
        var endsOnModel = new EndsOnModel { MaxOccurrences = 0 };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(FixedToday);
    }

    #region OnceRepeatingEntity

    private static readonly DateOnly OnceFirst = new(2026, 08, 15);
    private static readonly DateOnly OnceLast = new(2026, 09, 01);

    private static RepeatingEntityDto OnceModel() =>
        new(RepeatingEntityType.OnceRepeatingEntity, new OnceRepeatingEntity([OnceLast, OnceFirst]));

    [Fact]
    [Trait("OnceRepeatingEntity", "Derives EndsOn from the last chosen date")]
    public void GetScheduleEntity_Once_DerivesEndsOnFromLastDate()
    {
        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(OnceModel(), null, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(OnceLast);
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "Ignores a supplied EndsOnModel")]
    public void GetScheduleEntity_Once_IgnoresSuppliedEndsOnModel()
    {
        // A finite series defines its own end; walking it MaxOccurrences times would just run it dry.
        var endsOnModel = new EndsOnModel { MaxOccurrences = 99, MaxDate = new DateOnly(2027, 01, 01) };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(OnceModel(), endsOnModel, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(OnceLast);
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "A single chosen date becomes EndsOn")]
    public void GetScheduleEntity_Once_SingleDate_UsesThatDate()
    {
        var single = new DateOnly(2026, 08, 20);
        var model = new RepeatingEntityDto(RepeatingEntityType.OnceRepeatingEntity, new OnceRepeatingEntity([single]));

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(model, null, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(single);
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "A more restrictive MaxDate overrides the derived EndsOn")]
    public void GetScheduleEntity_Once_MoreRestrictiveMaxDate_OverridesEndsOn()
    {
        // Earlier than OnceLast, so the caller's bound is the tighter one and must win.
        var maxDate = OnceFirst.AddDays(2);
        var endsOnModel = new EndsOnModel { MaxDate = maxDate };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(OnceModel(), endsOnModel, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(maxDate);
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "A less restrictive MaxDate leaves the derived EndsOn alone")]
    public void GetScheduleEntity_Once_LessRestrictiveMaxDate_KeepsLastDate()
    {
        // Later than OnceLast: the series still ends when its dates run out, not at the caller's bound.
        var endsOnModel = new EndsOnModel { MaxDate = OnceLast.AddYears(1) };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(OnceModel(), endsOnModel, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(OnceLast);
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "A MaxOccurrences shorter than the list clamps EndsOn")]
    public void GetScheduleEntity_Once_MaxOccurrencesShorterThanList_ClampsToNthDate()
    {
        // Stopping after one occurrence lands on the first chosen date, ahead of the series' own end.
        var endsOnModel = new EndsOnModel { MaxOccurrences = 1 };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(OnceModel(), endsOnModel, FixedToday, FixedClock);

        scheduledEntity.EndsOn.Should().Be(OnceFirst);
    }

    [Theory]
    [InlineData(0)]   // the anchor day itself
    [InlineData(-1)]  // before the anchor
    [Trait("OnceRepeatingEntity", "A date at or before the anchor is rejected")]
    public void GetScheduleEntity_Once_DateNotAfterAnchor_Throws(int offsetFromAnchor)
    {
        var anchor = FixedToday.AddYears(2);
        var model = new RepeatingEntityDto(RepeatingEntityType.OnceRepeatingEntity, new OnceRepeatingEntity([anchor.AddDays(offsetFromAnchor), anchor.AddDays(10)]));

        var act = () => ScheduleEntityHelper.GetScheduleEntity(model, null, anchor, FixedClock);

        act.Should().Throw<DataIsNotCorrectException>();
    }

    [Fact]
    [Trait("OnceRepeatingEntity", "A date after the anchor but not after today is rejected")]
    public void GetScheduleEntity_Once_DateNotAfterToday_Throws()
    {
        // The anchor is in the past, so "today" is the binding floor: a date between them could never
        // be generated either.
        var anchor = FixedToday.AddDays(-30);
        var model = new RepeatingEntityDto(RepeatingEntityType.OnceRepeatingEntity, new OnceRepeatingEntity([FixedToday]));

        var act = () => ScheduleEntityHelper.GetScheduleEntity(model, null, anchor, FixedClock);

        act.Should().Throw<DataIsNotCorrectException>();
    }

    #endregion

    #region Anchoring

    [Fact]
    [Trait("GetScheduleEntity", "Both progress markers are seeded from the anchor date")]
    public void GetScheduleEntity_SeedsProgressMarkersFromAnchor()
    {
        // The parent entity already occupies the anchor day, so the series must resume strictly after it
        // rather than regenerating that day on top of the entity itself.
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(1));
        var anchor = FixedToday.AddDays(7);

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, null, anchor, FixedClock);

        scheduledEntity.FirstEntityCreated.Should().Be(anchor);
        scheduledEntity.LastEntityCreated.Should().Be(anchor);

        ScheduleEntityReturn.Create(scheduledEntity)
            .GetNextEntityDatesIn(FixedToday, anchor.AddDays(2))
            .Should().Equal(anchor.AddDays(1), anchor.AddDays(2));
    }

    [Fact]
    [Trait("GetScheduleEntity", "MaxOccurrences counts forward from the anchor, not from today")]
    public void GetScheduleEntity_MaxOccurrences_CountsFromAnchor()
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));
        var anchor = FixedToday.AddDays(10);
        var endsOnModel = new EndsOnModel { MaxOccurrences = 3 };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, anchor, FixedClock);

        scheduledEntity.EndsOn.Should().Be(anchor.AddDays(6));
    }

    #endregion

    #region EndsOnModel restrictiveness

    [Fact]
    [Trait("GetScheduleEntity", "A MaxDate later than the occurrence date does not override it")]
    public void GetScheduleEntity_MaxDateLaterThanOccurrence_KeepsOccurrenceDate()
    {
        // Mirror of GetScheduleEntity_MaxDateEarlierThanOccurrence_ClampsToMaxDate: the clamp is a Min,
        // so a looser MaxDate must be a no-op rather than pushing EndsOn out.
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));
        var endsOnModel = new EndsOnModel { MaxOccurrences = 3, MaxDate = FixedToday.AddMonths(6) };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedToday, FixedClock);

        // Day(2) stepped 3 times from 2024-01-01.
        scheduledEntity.EndsOn.Should().Be(new DateOnly(2024, 01, 07));
    }

    #endregion
}
