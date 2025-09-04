namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes
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
                _yearDayToRepeat = value switch
                {
                    > MaxYearDay => throw new ArgumentException(
                        $"Property value must not be greater than maximum days in year ({MaxYearDay})",
                        nameof(YearDayToRepeat)),
                    < 1 => throw new ArgumentException(
                        $"Property value must not be at least first day in month (1)",
                        nameof(YearDayToRepeat)),
                    _ => value
                };
            }
        }

        public YearRepeatingEntity()
        {}

        public YearRepeatingEntity(int yearDayToRepeat)
        {
            YearDayToRepeat = yearDayToRepeat;
        }

        public DateOnly GetNextTaskDate(DateOnly startingFrom)
        {
            const int maxIterations = 4;
            var startingDay = startingFrom.DayOfYear;
            startingFrom = startingFrom.AddDays(-startingFrom.Day + 1);
            startingFrom = startingFrom.AddMonths(-startingFrom.Month + 1);

            if (startingDay >= YearDayToRepeat)
                startingFrom = startingFrom.AddYears(1);

            for (var i = 0; i < maxIterations; i++)
            {
                var maxDayInYear = DateTime.IsLeapYear(startingFrom.Year) ? MaxYearDay : MaxYearDay - 1;
                if (maxDayInYear >= YearDayToRepeat)
                    return startingFrom.AddDays(YearDayToRepeat - 1);

                startingFrom = startingFrom.AddYears(1);
            }

            throw new Exception("No next task date found");
        }
    }
}
