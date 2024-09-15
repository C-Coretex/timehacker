namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public class DayRepeatingEntity: IRepeatingEntityType
    {
        private int DaysCountToRepeat { get; set; }

        public DayRepeatingEntity(int daysCountToRepeat)
        {
            DaysCountToRepeat = daysCountToRepeat;
        }

        public DateOnly GetNextTaskDate(DateOnly startingFrom)
        {
            return startingFrom.AddDays(DaysCountToRepeat);
        }
    }
}
