namespace TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes
{
    public interface IRepeatingEntityType
    {
        DateOnly GetNextTaskDate(DateOnly startingFrom);
    }
}
