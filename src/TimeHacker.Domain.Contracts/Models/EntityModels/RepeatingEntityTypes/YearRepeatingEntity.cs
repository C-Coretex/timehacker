namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public class YearRepeatingEntity: IRepeatingEntityType
    {
        private const int MaxYearDay = 366;

        private int _yearDayToRepeat;
        public int YearDayToRepeat
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

        public DateOnly GetNextTaskDate(DateOnly startingFrom)
        {
            const int maxIterations = 4;
            var startingDay = startingFrom.DayOfYear;
            startingFrom.AddDays(-startingFrom.Day);

            if (startingDay > YearDayToRepeat)
                startingFrom.AddYears(1);

            for (var i = 0; i < maxIterations; i++)
            {
                var maxDayInYear = DateTime.IsLeapYear(startingFrom.Year) ? MaxYearDay : MaxYearDay - 1;
                if (maxDayInYear >= YearDayToRepeat)
                    return startingFrom.AddDays(YearDayToRepeat);

                startingFrom.AddYears(1);
            }

            throw new Exception("No next task date found");
        }
    }
}
