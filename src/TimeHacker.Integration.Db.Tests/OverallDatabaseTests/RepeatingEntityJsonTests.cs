using Microsoft.EntityFrameworkCore;
using Timehacker.Integration.Db.Tests;
using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Integration.Db.Tests.Fixtures;
using DayOfWeekEnum = TimeHacker.Domain.Models.EntityModels.Enums.DayOfWeek;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

// ScheduleEntity.RepeatingEntity is persisted as polymorphic JSON bytes. These tests prove every
// IRepeatingEntityType derived type survives the serialize/deserialize round-trip with its concrete
// runtime type intact.
public class RepeatingEntityJsonTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Theory]
    [InlineData(RepeatingEntityType.DayRepeatingEntity)]
    [InlineData(RepeatingEntityType.WeekRepeatingEntity)]
    [InlineData(RepeatingEntityType.MonthRepeatingEntity)]
    [InlineData(RepeatingEntityType.YearRepeatingEntity)]
    [Trait("RepeatingEntity", "PolymorphicJsonRoundTrip")]
    public async Task RepeatingEntity_Should_RoundTripPolymorphicJson(RepeatingEntityType type)
    {
        var dto = BuildDto(type);
        var entity = new ScheduleEntity { RepeatingEntity = dto };
        await Resolve<IScheduleEntityRepository>().AddAndSaveAsync(entity, TestContext.Current.CancellationToken);

        Db.ChangeTracker.Clear();
        var reloaded = await Db.Set<ScheduleEntity>().FirstAsync(x => x.Id == entity.Id, TestContext.Current.CancellationToken);

        reloaded.RepeatingEntity.EntityType.Should().Be(type);
        reloaded.RepeatingEntity.RepeatingData.Should().BeOfType(dto.RepeatingData.GetType());
        AssertPayloadPreserved(dto.RepeatingData, reloaded.RepeatingEntity.RepeatingData);
    }

    private static void AssertPayloadPreserved(IRepeatingEntityType expected, IRepeatingEntityType actual)
    {
        // RepeatingData is declared as the IRepeatingEntityType interface (no members), so assert the
        // concrete payload of whichever derived type round-tripped.
        switch (actual)
        {
            case DayRepeatingEntity day:
                day.DaysCountToRepeat.Should().Be(((DayRepeatingEntity)expected).DaysCountToRepeat);
                break;
            case WeekRepeatingEntity week:
                week.RepeatsOn.Should().Equal(((WeekRepeatingEntity)expected).RepeatsOn);
                break;
            case MonthRepeatingEntity month:
                month.MonthDayToRepeat.Should().Be(((MonthRepeatingEntity)expected).MonthDayToRepeat);
                break;
            case YearRepeatingEntity year:
                year.YearDayToRepeat.Should().Be(((YearRepeatingEntity)expected).YearDayToRepeat);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actual), actual, null);
        }
    }

    private static RepeatingEntityDto BuildDto(RepeatingEntityType type) => type switch
    {
        RepeatingEntityType.DayRepeatingEntity => new(type, new DayRepeatingEntity(3)),
        RepeatingEntityType.WeekRepeatingEntity => new(type, new WeekRepeatingEntity([DayOfWeekEnum.Monday, DayOfWeekEnum.Friday])),
        RepeatingEntityType.MonthRepeatingEntity => new(type, new MonthRepeatingEntity(15)),
        RepeatingEntityType.YearRepeatingEntity => new(type, new YearRepeatingEntity(200)),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
