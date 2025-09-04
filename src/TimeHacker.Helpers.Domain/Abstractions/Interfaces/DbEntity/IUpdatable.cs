namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity
{
    public interface IUpdatable
    {
        DateTime? UpdatedTimestamp { get; set; }
    }
}
