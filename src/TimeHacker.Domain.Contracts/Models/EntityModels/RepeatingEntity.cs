using TimeHacker.Domain.Contracts.Models.EntityModels.Enums;
using TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Domain.Contracts.Models.EntityModels
{
    public class RepeatingEntity
    {
        public RepeatingEntityTypeEnum EntityType { get; set; }

        private object _repeatingData;

        public object RepeatingData
        {
            get => _repeatingData;
            set
            {
                if (value is IRepeatingEntityType)
                    _repeatingData = value;
                else
                    throw new Exception("Value must implement RepeatingEntityTypeBase.");
            }
        }
    }
}
