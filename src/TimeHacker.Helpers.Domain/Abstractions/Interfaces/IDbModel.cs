using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Helpers.Domain.Abstractions.Interfaces
{
    public interface IDbModel<T>
    {
        [Key]
        public T Id { get; init; }
    }
}
