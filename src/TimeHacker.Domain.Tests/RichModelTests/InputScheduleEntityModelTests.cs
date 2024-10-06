using FluentAssertions;
using TimeHacker.Domain.Contracts.Models.EntityModels;
using TimeHacker.Domain.Contracts.Models.EntityModels.Enums;
using TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Domain.Tests.RichModelTests
{
    public class InputScheduleEntityModelTests
    {
        [Fact]
        [Trait("DayRepeatingEntity", "Should return correct data without EndsOnModel")]
        public void GetScheduleEntity_ShouldReturnCorrectDataWithoutEndsOnModel()
        {
            var repeatingEntityModel = new RepeatingEntityModel()
            {
                EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                RepeatingData = new DayRepeatingEntity()
            };

            var model = new InputScheduleEntityModel()
            {
                RepeatingEntityModel = repeatingEntityModel,
                EndsOnModel = null
            };

            var scheduledEntity = model.GetScheduleEntity();
            scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);
            scheduledEntity.EndsOn.Should().BeNull();
        }

        [Fact]
        [Trait("DayRepeatingEntity", "Should return correct data with EndsOnModel without MaxOccurrences")]
        public void GetScheduleEntity_ShouldReturnCorrectDataWithEndsOnModelWithoutMaxOccurrences()
        {
            var repeatingEntityModel = new RepeatingEntityModel()
            {
                EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                RepeatingData = new DayRepeatingEntity()
            };

            var maxDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
            var model = new InputScheduleEntityModel()
            {
                RepeatingEntityModel = repeatingEntityModel,
                EndsOnModel = new EndsOnModel()
                {
                    MaxDate = maxDate,
                    MaxOccurrences = null
                }
            };

            var scheduledEntity = model.GetScheduleEntity();
            scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);
            scheduledEntity.EndsOn.Should().Be(maxDate);
        }

        [Theory, CombinatorialData]
        [Trait("DayRepeatingEntity", "Should return correct data with EndsOnModel with MaxOccurrences")]
        public void GetScheduleEntity_ShouldReturnCorrectDataWithEndsOnModelWithMaxOccurrences([CombinatorialValues(0, 1, 5, 10)] uint maxOccurrences, bool isMaxDate)
        {
            var repeatingEntityModel = new RepeatingEntityModel()
            {
                EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                RepeatingData = new DayRepeatingEntity(2)
            };

            var maxDate = isMaxDate ? (DateOnly?)DateOnly.FromDateTime(DateTime.Now.AddDays(8)) : null;
            var model = new InputScheduleEntityModel()
            {
                RepeatingEntityModel = repeatingEntityModel,
                EndsOnModel = new EndsOnModel()
                {
                    MaxDate = maxDate,
                    MaxOccurrences = maxOccurrences
                }
            };

            var scheduledEntity = model.GetScheduleEntity();
            scheduledEntity.RepeatingEntity.Should().Be(repeatingEntityModel);

            var endsOn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(maxOccurrences * 2));

            if (isMaxDate && endsOn > maxDate)
                endsOn = maxDate.Value;
            scheduledEntity.EndsOn.Should().Be(endsOn);
        }
    }
}
