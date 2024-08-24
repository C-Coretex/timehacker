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

        public DateOnly GetNextTaskDate(DateOnly startingFrom)
        {
            const int maxIterations = 12;
            var startingDay = startingFrom.Day;

            //Reset to 1-st day
            startingFrom.AddDays(-startingFrom.Day);
            if (startingDay > MonthDayToRepeat)
                startingFrom.AddMonths(1);

            for (var i = 0; i < maxIterations; i++)
            {
                var maxDayInMonth = DateTime.DaysInMonth(startingFrom.Year, startingFrom.Month);
                if (maxDayInMonth >= MonthDayToRepeat)
                    return startingFrom.AddDays(MonthDayToRepeat);

                startingFrom.AddMonths(1);
            }

            throw new Exception("No next task date found");
        }
    }
}
