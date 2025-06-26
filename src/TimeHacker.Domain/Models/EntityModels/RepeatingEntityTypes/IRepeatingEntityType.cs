namespace TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes
{
    public interface IRepeatingEntityType
    {
        DateOnly GetNextTaskDate(DateOnly startingFrom);
    }
}
