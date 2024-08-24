namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public class YearRepeatingEntity
    {
        private const byte MaxYearDay = 31;

        private byte _yearDayToRepeat;
        public byte YearDayToRepeat
        {
            get => _yearDayToRepeat;
            set
            {
                if (value > MaxYearDay)
                    throw new ArgumentException(
                        $"Property value must not be greater than maximum days in year ({MaxYearDay})",
                        nameof(YearDayToRepeat));

                _yearDayToRepeat = value;
            }
        }
    }
}
