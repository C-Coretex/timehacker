namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public class DayRepeatingEntity: IRepeatingEntityType
    {
        private byte DaysCountToRepeat { get; set; }

        public DayRepeatingEntity(byte daysCountToRepeat)
        {
            DaysCountToRepeat = daysCountToRepeat;
        }

        public DateOnly GetNextTaskDate(DateOnly startingFrom)
        {
            return startingFrom.AddDays(DaysCountToRepeat);
        }
    }
}
