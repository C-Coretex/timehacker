﻿namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity
{
    public interface IDbEntity<T>: IDbEntity
    {
        public T Id { get; init; }
    }

    public interface IDbEntity
    {
    }
}
