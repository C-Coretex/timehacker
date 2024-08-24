namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public class MonthRepeatingEntity: IRepeatingEntityType
    {
        private const byte MaxMonthDay = 31;

        private byte _monthDayToRepeat;
        public byte MonthDayToRepeat
        {
            get => _monthDayToRepeat;
            set
            {
                if (value > MaxMonthDay)
                    throw new ArgumentException(
                        $"Property value must not be greater than maximum days in month ({MaxMonthDay})",
                        nameof(MonthDayToRepeat));

                _monthDayToRepeat = value;
            }
        }
    }
}
