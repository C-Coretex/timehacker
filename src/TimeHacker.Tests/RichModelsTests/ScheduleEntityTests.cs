using FluentAssertions;
using TimeHacker.Domain.Contracts.Models.EntityModels;
using TimeHacker.Domain.Contracts.Models.EntityModels.Enums;
using TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Contracts.Models.ReturnModels;
using TimeHacker.Tests.Helpers;

namespace TimeHacker.Tests.RichModelsTests
{
    public class ScheduleEntityTests
    {
        #region DayRepeatingEntity

        [Theory]
        [MemberData(nameof(TheoryDataHelpers.TwoBoolPermutationData), MemberType = typeof(TheoryDataHelpers))]
        [Trait("DayRepeatingEntity", "Should return correct data")]
        public void DayRepeatingEntity_ShouldReturnCorrectData(bool endsOn, bool lastEntityCreated)
        {
            var newEntity = new ScheduleEntityReturn()
            {
                RepeatingEntity = new RepeatingEntityModel()
                {
                    EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                    RepeatingData = new DayRepeatingEntity(2)
                },
                LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(DateTime.Now.AddDays(1)) : null,
                EndsOn = endsOn ? DateOnly.FromDateTime(DateTime.Now.AddDays(6)) : null
            };
            var dateFrom = DateOnly.FromDateTime(DateTime.Now);
            var dateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(10));

            var result = newEntity.GetNextEntityDatesIn(dateFrom, dateTo).ToList();
            Assert.NotNull(result);

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
                    RepeatingEntity = new RepeatingEntityModel()
                    {
                        EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                        RepeatingData = new DayRepeatingEntity(0)
                    },
                    LastEntityCreated = null,
                    EndsOn = null
                };
            });
        }

        #endregion

        #region WeekRepeatingEntity

        [Theory]
        [MemberData(nameof(TheoryDataHelpers.TwoBoolPermutationData), MemberType = typeof(TheoryDataHelpers))]
        [Trait("WeekRepeatingEntity", "Should return correct data")]
        public void WeekRepeatingEntity_ShouldReturnCorrectData(bool endsOn, bool lastEntityCreated)
        {
            var monday = new DateTime(2024, 09, 16);

            var newEntity = new ScheduleEntityReturn()
            {
                RepeatingEntity = new RepeatingEntityModel()
                {
                    EntityType = RepeatingEntityTypeEnum.WeekRepeatingEntity,
                    RepeatingData = new WeekRepeatingEntity([DayOfWeekEnum.Monday, DayOfWeekEnum.Tuesday, DayOfWeekEnum.Friday])
                },
                CreatedTimestamp = monday,
                LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(monday.AddDays(1)) : null,
                EndsOn = endsOn ? DateOnly.FromDateTime(monday.AddDays(8)) : null
            };
            var dateFrom = DateOnly.FromDateTime(monday);
            var dateTo = DateOnly.FromDateTime(monday.AddDays(14));

            var result = newEntity.GetNextEntityDatesIn(dateFrom, dateTo).ToList();
            Assert.NotNull(result);

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
                    RepeatingEntity = new RepeatingEntityModel()
                    {
                        EntityType = RepeatingEntityTypeEnum.WeekRepeatingEntity,
                        RepeatingData = new WeekRepeatingEntity([])
                    },
                    LastEntityCreated = null,
                    EndsOn = null
                };
            });
        }

        #endregion

    }
}
