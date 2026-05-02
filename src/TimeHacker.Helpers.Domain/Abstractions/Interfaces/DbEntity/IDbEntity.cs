namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

public interface IDbEntity<T>: IDbEntity
{
    public T Id { get; init; }
}

#pragma warning disable CA1040 // Avoid empty interfaces
public interface IDbEntity
#pragma warning restore CA1040 // Avoid empty interfaces
{
}
