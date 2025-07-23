using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Domain.Models.EntityModels
{
    public class RepeatingEntityModel
    {
        public required RepeatingEntityTypeEnum EntityType { get; init; }

        //private object _repeatingData;

        public required IRepeatingEntityType RepeatingData { get; init; }
        /*{
            get => _repeatingData;
            set
            {
                if (value is IRepeatingEntityType)
                    _repeatingData = value;
                else
                    throw new Exception("Value must implement RepeatingEntityTypeBase.");
            }
        }*/
    }
}
