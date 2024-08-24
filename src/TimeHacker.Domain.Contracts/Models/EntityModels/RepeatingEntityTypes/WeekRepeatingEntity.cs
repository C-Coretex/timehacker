using TimeHacker.Domain.Contracts.Models.EntityModels.Enums;

namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public class WeekRepeatingEntity: IRepeatingEntityType
    {
        public IEnumerable<RepeatsOnEnum> RepeatsOn = [];
    }
}
