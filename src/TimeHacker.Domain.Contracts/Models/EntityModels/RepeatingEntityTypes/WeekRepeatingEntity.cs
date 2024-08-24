using TimeHacker.Domain.Contracts.Models.EntityModels.Enums;
using TimeHacker.Helpers.Domain.Extensions;

namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public class WeekRepeatingEntity: IRepeatingEntityType
    {
        private IEnumerable<DayOfWeekEnum> _repeatsOn = [];

        public IEnumerable<DayOfWeekEnum> RepeatsOn
        {
            get => _repeatsOn;
            set
            {
                if (!value.Any())
                    throw new ArgumentException("At least one day of week must be chosen", nameof(RepeatsOn));

                _repeatsOn = value.OrderBy(x => x).ToList(); 
            }
        }

        public DateOnly GetNextTaskDate(DateOnly startingFrom)
        {
            var currentDayOfWeek = (int)startingFrom.DayOfWeek;

            //Sunday is 7 in DayOfWeekEnum
            if (currentDayOfWeek == 0)
                currentDayOfWeek = 7;

            var nextDayOfWeek = (int?)RepeatsOn.FirstOrNull(x => (int)x > currentDayOfWeek);
            var daysToAdd = 0;
            if (!nextDayOfWeek.HasValue)
            {
                nextDayOfWeek = (int)RepeatsOn.First();
                //Add last day of week, since we need to travel from currentDayOfWeek to the end of the week
                daysToAdd += (int)DayOfWeekEnum.Sunday;
            }

            daysToAdd += nextDayOfWeek.Value - currentDayOfWeek;

            return startingFrom.AddDays(daysToAdd);
        }
    }
}
