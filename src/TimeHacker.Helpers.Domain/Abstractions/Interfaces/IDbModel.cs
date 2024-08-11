namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces
{
    public interface IDbModel<T>: IDbModel
    {
        public T Id { get; init; }
    }

    public interface IDbModel
    {
    }
}
