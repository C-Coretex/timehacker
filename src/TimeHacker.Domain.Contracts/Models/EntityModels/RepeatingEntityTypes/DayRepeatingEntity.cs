namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public class DayRepeatingEntity: IRepeatingEntityType
    {
        public byte DaysCountToRepeat { get; set; }

        public DayRepeatingEntity(byte daysCountToRepeat)
        {
            DaysCountToRepeat = daysCountToRepeat;
        }
    }
}
