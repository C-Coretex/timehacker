using System.ComponentModel.DataAnnotations;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Api.Models.Input.Tasks.RepeatingEntities
{
    public record InputYearRepeatingEntityModel : IInputRepeatingEntityType
    {
        [Required]
        public int YearDayToRepeat { get; set; }
        public RepeatingEntityTypeEnum EntityType { get; init; } = RepeatingEntityTypeEnum.YearRepeatingEntity;

        public IRepeatingEntityType CreateEntity()
        {
            return new YearRepeatingEntity(YearDayToRepeat);
        }
    }
}
