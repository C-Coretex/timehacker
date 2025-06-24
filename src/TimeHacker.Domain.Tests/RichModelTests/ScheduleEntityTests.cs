using AwesomeAssertions;
using TimeHacker.Domain.Contracts.Models.EntityModels;
using TimeHacker.Domain.Contracts.Models.EntityModels.Enums;
using TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Domain.Tests.RichModelTests
{
    public class ScheduleEntityTests
    {
        #region DayRepeatingEntity

        [Theory, CombinatorialData]
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

        [Theory, CombinatorialData]
        [Trait("WeekRepeatingEntity", "Should return correct data")]
        public void WeekRepeatingEntity_ShouldReturnCorrectData(bool endsOn, bool lastEntityCreated)
        {
            var monday = new DateTime(2024, 09, 16);

            var newEntity = new ScheduleEntityReturn()
            {
                RepeatingEntity = new RepeatingEntityModel()
                {
                    EntityType = RepeatingEntityTypeEnum.WeekRepeatingEntity,
                    RepeatingData = new WeekRepeatingEntity([DayOfWeekEnum.Tuesday, DayOfWeekEnum.Monday, DayOfWeekEnum.Friday])
                },
                CreatedTimestamp = monday,
                LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(monday.AddDays(1)) : null,
                EndsOn = endsOn ? DateOnly.FromDateTime(monday.AddDays(8)) : null
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

        #region MonthRepeatingEntity

        [Theory, CombinatorialData]
        [Trait("MonthRepeatingEntity", "Should return correct data")]
        public void MonthRepeatingEntity_ShouldReturnCorrectData(bool endsOn, bool lastEntityCreated)
        {
            var january = new DateTime(2023, 01, 01);

            var newEntity = new ScheduleEntityReturn()
            {
                RepeatingEntity = new RepeatingEntityModel()
                {
                    EntityType = RepeatingEntityTypeEnum.MonthRepeatingEntity,
                    RepeatingData = new MonthRepeatingEntity(12)
                },
                CreatedTimestamp = january,
                LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddDays(12)) : null,
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
                RepeatingEntity = new RepeatingEntityModel()
                {
                    EntityType = RepeatingEntityTypeEnum.MonthRepeatingEntity,
                    RepeatingData = new MonthRepeatingEntity((byte)(isLapYear ? 29 : 31))
                },
                CreatedTimestamp = january,
                LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddDays(31)) : null,
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
                    RepeatingEntity = new RepeatingEntityModel()
                    {
                        EntityType = RepeatingEntityTypeEnum.MonthRepeatingEntity,
                        RepeatingData = new MonthRepeatingEntity(days)
                    },
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
                RepeatingEntity = new RepeatingEntityModel()
                {
                    EntityType = RepeatingEntityTypeEnum.YearRepeatingEntity,
                    RepeatingData = new YearRepeatingEntity(isLapYear ? 366 : 200)
                },
                CreatedTimestamp = january,
                LastEntityCreated = lastEntityCreated ? DateOnly.FromDateTime(january.AddYears(1)) : null,
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
                    RepeatingEntity = new RepeatingEntityModel()
                    {
                        EntityType = RepeatingEntityTypeEnum.YearRepeatingEntity,
                        RepeatingData = new YearRepeatingEntity(days)
                    },
                    LastEntityCreated = null,
                    EndsOn = null
                };
            });
        }

        #endregion
    }
}
