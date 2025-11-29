namespace TimeHacker.Application.Api.Tests.RichModelTests
{
    public class InputScheduleEntityModelTests
    {
        [Fact]
        [Trait("DayRepeatingEntity", "Should return correct data without EndsOnModel")]
        public void GetScheduleEntity_ShouldReturnCorrectDataWithoutEndsOnModel()
        {
            var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityTypeEnum.DayRepeatingEntity, new DayRepeatingEntity());

            var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, null);
            scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);
            scheduledEntity.EndsOn.Should().BeNull();
        }

        [Fact]
        [Trait("DayRepeatingEntity", "Should return correct data with EndsOnModel without MaxOccurrences")]
        public void GetScheduleEntity_ShouldReturnCorrectDataWithEndsOnModelWithoutMaxOccurrences()
        {
            var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityTypeEnum.DayRepeatingEntity, new DayRepeatingEntity());

            var maxDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
            var endsOnModel = new EndsOnModel()
            {
                MaxDate = maxDate,
                MaxOccurrences = null
            };

            var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel);
            scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);
            scheduledEntity.EndsOn.Should().Be(maxDate);
        }

        [Theory, CombinatorialData]
        [Trait("DayRepeatingEntity", "Should return correct data with EndsOnModel with MaxOccurrences")]
        public void GetScheduleEntity_ShouldReturnCorrectDataWithEndsOnModelWithMaxOccurrences([CombinatorialValues(0, 1, 5, 10)] uint maxOccurrences, bool isMaxDate)
        {
            var repeatingEntityModel = new RepeatingEntityDto(RepeatingEntityTypeEnum.DayRepeatingEntity, new DayRepeatingEntity(2));

            var maxDate = isMaxDate ? (DateOnly?)DateOnly.FromDateTime(DateTime.Now.AddDays(8)) : null;
            var endsOnModel = new EndsOnModel()
            {
                MaxDate = maxDate,
                MaxOccurrences = maxOccurrences
            };

            var scheduledEntity = ScheduleEntityHelper.GetScheduleEntity(repeatingEntityModel, endsOnModel);
            scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);

            var endsOn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(maxOccurrences * 2));

            if (isMaxDate && endsOn > maxDate)
                endsOn = maxDate.Value;
            scheduledEntity.EndsOn.Should().Be(endsOn);
        }
    }
}
