using System.ComponentModel.DataAnnotations;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities
{
    public record InputDayRepeatingEntityModel : IInputRepeatingEntityType
    {
        [Required]
        public int DaysCountToRepeat { get; set; }
        public RepeatingEntityTypeEnum EntityType { get; init; } = RepeatingEntityTypeEnum.DayRepeatingEntity;

        public IRepeatingEntityType CreateEntity()
        {
            return new DayRepeatingEntity(DaysCountToRepeat);
        }
    }
}
