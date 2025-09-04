using System.ComponentModel.DataAnnotations;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities
{
    public record InputMonthRepeatingEntityModel : IInputRepeatingEntityType
    {
        [Required]
        public byte MonthDayToRepeat { get; init; }
        public RepeatingEntityTypeEnum EntityType { get; init; } = RepeatingEntityTypeEnum.MonthRepeatingEntity;

        public IRepeatingEntityType CreateEntity()
        {
            return new MonthRepeatingEntity(MonthDayToRepeat);
        }
    }
}
