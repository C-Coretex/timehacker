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

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, null, TimeProvider.System);
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

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, TimeProvider.System);
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

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, TimeProvider.System);
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

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedClock);

        scheduledEntity.EndsOn.Should().Be(expectedEndsOn);
    }

    [Fact]
    [Trait("GetScheduleEntity", "MaxDate earlier than the occurrence date clamps EndsOn")]
    public void GetScheduleEntity_MaxDateEarlierThanOccurrence_ClampsToMaxDate()
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));
        var maxDate = FixedToday.AddDays(5);
        var endsOnModel = new EndsOnModel { MaxOccurrences = 10, MaxDate = maxDate };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedClock);

        scheduledEntity.EndsOn.Should().Be(maxDate);
    }

    [Theory]
    [InlineData(true), InlineData(false)]
    [Trait("GetScheduleEntity", "Empty EndsOnModel (no MaxOccurrences, no MaxDate) yields null EndsOn")]
    public void GetScheduleEntity_EmptyEndsOnModel_YieldsNullEndsOn(bool includeEndsOnModel)
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));
        var endsOnModel = includeEndsOnModel ? new EndsOnModel { MaxOccurrences = null, MaxDate = null } : null;

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedClock);

        scheduledEntity.EndsOn.Should().BeNull();
    }

    [Fact]
    [Trait("GetScheduleEntity", "Zero MaxOccurrences yields today")]
    public void GetScheduleEntity_ZeroMaxOccurrences_YieldsToday()
    {
        var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityType.DayRepeatingEntity, new DayRepeatingEntity(2));
        var endsOnModel = new EndsOnModel { MaxOccurrences = 0 };

        var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel, FixedClock);

        scheduledEntity.EndsOn.Should().Be(FixedToday);
    }
}
